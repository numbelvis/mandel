#include <stdio.h>
#include <inttypes.h>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"


#ifndef CONFIG_DEFINED
#define CONFIG_DEFINED

// Number of real digits per Digit stored.
static const int PRECISION = 4;


// 10 ^ PRECISION: Used in many calcs.
static const int MAGNITUDE = 10000;


// Maximum number of Digits that will be stored per integer or decimal.
static const int DIGITS = 25;
static const int DMO = DIGITS - 1;			// DIGITS MINUS ONE.  Used in loops and indexing.

static const int HUGE_DIGITS = DIGITS * 2; // DIGITS TIMES TWO
static const int HDMO = HUGE_DIGITS - 1;			// HUGE DIGITS MINUS ONE.  Used in loops and indexing.

#endif



#ifndef TYPES_DEFINED
#define TYPES_DEFINED

// An arbitrary precision integer.
struct ArbInt
{
	bool sign;
	uint16_t digits[DIGITS];
};

// An arbitrary int used for multiplication calcs.
struct ArbHugeInt
{
	bool sign;
	uint16_t digits[DIGITS * 2];
};

// An arbitrary precision decimal.
struct ArbDec
{
	bool sign = true;
	uint16_t digits[DIGITS];
	int decpos = 0;
};

#endif

__device__ void PrintArb(ArbDec *num)
{
	if (num->sign == false)
		printf("-");

	bool started = false;
	bool first = true;
	for (int ii = 0; ii < num->decpos; ii++)
	{
		if (started == false && num->digits[ii] != 0)
			started = true;

		if (started)
		{
			if (first)
			{
				first = false;
				printf("%d", num->digits[ii]);
			}
			else
				printf("%04d", num->digits[ii]);
		}
	}

	int endat = num->decpos;
	for (int ii = DMO; ii >= num->decpos; ii--)
	{
		if (num->digits[ii] != 0)
		{
			endat = ii;
			if (endat < 0)
				endat = 0;

			break;
		}
	}

	if (endat > num->decpos - 1)
	{
		printf(".");
		for (int ii = num->decpos; ii <= endat; ii++)
		{
			printf("%04d", num->digits[ii]);
		}
	}
}


/**************** Zero-related ****************************************************/

/*
Zero out all elements in an Arb number.
*/
__device__ void ZeroOut(ArbInt *arb)
{
	for (int kk = 0; kk < DIGITS; kk++)
		arb->digits[kk] = 0;
}

__device__ void ZeroOut(ArbDec *arb)
{
	for (int kk = 0; kk < DIGITS; kk++)
		arb->digits[kk] = 0;
}

__device__ void ZeroOut(ArbHugeInt *arb)
{
	for (int kk = 0; kk < HUGE_DIGITS; kk++)
		arb->digits[kk] = 0;
}


/*
Retrieve a zero value.
*/
__device__ ArbInt GetZeroInt()
{
	struct ArbInt arb;
	arb.sign = true;
	ZeroOut(&arb);
	return arb;
}

__device__ ArbDec GetZeroDec()
{
	struct ArbDec arb;
	arb.sign = true;
	arb.decpos = 0;
	ZeroOut(&arb);
	return arb;
}



/******************* Convert from regular number types or between arb types ******************************/

__device__ ArbInt ArbFromInteger(int num)
{
	struct ArbInt arb = GetZeroInt();

	// Determine the sign.
	arb.sign = num >= 0;

	// Get rid of the sign.
	if (num < 0)
		num = -num;

	for (int ii = DIGITS - 1; ii >= 0; ii--)
	{
		int this_digit = num % MAGNITUDE;
		arb.digits[ii] = this_digit;
		num = num / MAGNITUDE;
	}

	return arb;
}



__device__ ArbDec ArbFromDouble(double num)
{
	// Start with an int.
	int as_int = (int)num;
	ArbInt i = ArbFromInteger(as_int);

	int zero = -1;
	double rest = num - (double)as_int;

	struct ArbDec arb = GetZeroDec();

	arb.sign = num >= 0;

	for (int kk = 0; kk < DIGITS; kk++)
	{
		arb.digits[kk] = 0;
		if (i.digits[kk] != 0)
			zero = kk;
	}

	if (zero == -1)
	{
		// This number has no whole part.
		arb.decpos = 0;
		zero = DIGITS;
	}

	// Copy the int digits over
	int numofnonzero = DIGITS - zero;
	for (int kk = 0; kk < numofnonzero; kk++)
	{
		arb.digits[kk] = i.digits[kk + zero];
	}

	arb.decpos = numofnonzero;


	// We go 8 * magnitude deep.

	// Get rid of the sign
	if (rest < 0)
		rest = -rest;

	for (int ii = 0; ii < 8; ii++)
	{
		rest = rest * MAGNITUDE;
		int part = (int)rest;
		arb.digits[ii + numofnonzero] = part;

		rest = rest - (double)part;
	}

	return arb;
}


__device__ ArbDec Construct(bool sign, int decpos, uint16_t *copyFromDigits)
{
	struct ArbDec arb;
	arb.sign = sign;
	arb.decpos = decpos;

	for (int kk = 0; kk < DIGITS; kk++)
		arb.digits[kk] = copyFromDigits[kk];

	return arb;
}



/************************ Comparison of Arb Number ******************************/

__device__ bool GreaterThan(ArbDec *a, ArbDec *b)
{
	if (a->decpos == b->decpos)
	{

		for (int ii = 0; ii < DIGITS; ii++)
			if (a->digits[ii] != b->digits[ii])
				return a->digits[ii] > b->digits[ii];
	}
	else
	{
		return a->decpos > b->decpos;
	}

	return false;
}


__device__ bool GreaterThan(ArbInt *a, ArbInt *b)
{
	for (int ii = 0; ii < DIGITS; ii++)
		if (a->digits[ii] != b->digits[ii])
			return a->digits[ii] > b->digits[ii];

	return false;
}

__device__ bool GreaterThan(ArbHugeInt *a, ArbHugeInt *b)
{
	for (int ii = 0; ii < HUGE_DIGITS; ii++)
		if (a->digits[ii] != b->digits[ii])
			return a->digits[ii] > b->digits[ii];

	return false;
}





/************************************ Mathematical Operations on Arb numbers **************************************************************/

__device__ void AddHugePositiveIntegers(ArbHugeInt *a, ArbHugeInt *b, ArbHugeInt *result)
{
	uint16_t carry = 0;

	result->sign = true;
	bool all_zeros = true;
	for (int ii = 0; ii < HUGE_DIGITS; ii++)
	{
		result->digits[ii] = 0;
		if (all_zeros == true && (a->digits[ii] != 0 || b->digits[ii] != 0))
			all_zeros = false;
	}

	if (all_zeros)
	{
		result->sign = true;
		return;
	}

	for (int ii = HUGE_DIGITS - 1; ii >= 0; ii--)
	{
		int sum = a->digits[ii] + b->digits[ii] + carry;
		if (sum >= MAGNITUDE)
		{
			sum = sum - MAGNITUDE;
			carry = 1;
		}
		else
			carry = 0;

		result->digits[ii] = sum;
	}
}


// Multiply a and b.
__device__ void MultiplyHugePositiveIntegers(ArbInt *a, ArbInt *b, ArbHugeInt *result, ArbHugeInt *each_line, ArbHugeInt *sum_result)
{
	uint16_t carry = 0;
	bool a_on_top = GreaterThan(a, b);
	//	struct ArbHugeInt result;
	for (int kk = 0; kk < HUGE_DIGITS; kk++)
	{
		result->digits[kk] = 0;
		sum_result->digits[kk] = 0;
	}

	int a_size = -1;
	for (int kk = 0; kk < DIGITS; kk++)
	{
		if (a->digits[kk] != 0)
		{
			a_size = kk;
			break;
		}
	}

	int b_size = -1;
	for (int kk = 0; kk < DIGITS; kk++)
	{
		if (b->digits[kk] != 0)
		{
			b_size = kk;
			break;
		}
	}

	if (a_size == -1 || b_size == -1)
	{
		result->sign = true;
		return;
	}


	a_size -= 1;
	b_size -= 1;
	int top_size = (a_on_top ? a_size : b_size);
	int bot_size = (a_on_top ? b_size : a_size);
	for (int ii = DIGITS - 1; ii > bot_size; ii--)
	{
		int idx_move = DIGITS - ii - 1;
		//struct ArbHugeInt thisline;
		for (int kk = 0; kk < HUGE_DIGITS; kk++)
			each_line->digits[kk] = 0;

		each_line->sign = true;

		for (int jj = DIGITS - 1; jj > top_size; jj--)
		{
			int prod = (a_on_top ? (a->digits[jj] * b->digits[ii]) : (a->digits[ii] * b->digits[jj])) + carry;
			if (prod >= MAGNITUDE)
			{
				carry = prod / MAGNITUDE;
				prod = prod - (carry * MAGNITUDE);
			}
			else
			{
				carry = 0;
			}

			int idx = jj - idx_move + DIGITS;
			each_line->digits[idx] = prod;
		}

		if (carry > 0)
		{
			int idx = top_size - idx_move + DIGITS;
			each_line->digits[idx] = carry;
		}

		AddHugePositiveIntegers(sum_result, each_line, result);

		// Copy result back onto sum_result.
		for (int kk = 0; kk < HUGE_DIGITS; kk++)
		{
			sum_result->digits[kk] = result->digits[kk];
		}
	}

	bool a_sign = a->sign;
	bool b_sign = b->sign;
	if (a_sign == false && b_sign == false)
		result->sign = true;
	else if (a_sign == false || b_sign == false)
		result->sign = false;
}



__device__ void AddTwoPositiveArbitrary(ArbDec *a, ArbDec *b, ArbDec *result)
{
	uint16_t carry = 0;
	//struct ArbDec result;
	for (int ii = 0; ii < DIGITS; ii++)
		result->digits[ii] = 0;

	result->sign = true;
	result->decpos = a->decpos > b->decpos ? a->decpos : b->decpos;

	int pos_diff = a->decpos - b->decpos;
	bool a_on_top = GreaterThan(a, b);
	if (pos_diff < 0)
		pos_diff = -pos_diff;

	for (int ii = DMO; ii >= 0; ii--)
	{
		int bot_idx = ii - pos_diff;
		int sum = carry + (a_on_top ? a->digits[ii] : b->digits[ii]) + (bot_idx < 0 ? 0 : (a_on_top ? b->digits[bot_idx] : a->digits[bot_idx]));
		if (sum >= MAGNITUDE)
		{
			carry = 1;
			sum = sum - MAGNITUDE;
		}
		else
			carry = 0;

		result->digits[ii] = sum;
	}

	if (carry > 0)
	{
		// Shift the array right to make room for the carry.
		for (int jj = DMO - 1; jj > 0; jj--)
		{
			result->digits[jj] = result->digits[jj - 1];
		}
		result->digits[0] = carry;
		result->decpos = result->decpos + 1;
	}
}


__device__ void SubtractTwoPositiveArbitrary(ArbDec *a, ArbDec *b, ArbDec *result)
{
	uint16_t take = 0;
	//struct ArbDec result;
	bool all_zeros = true;
	for (int ii = 0; ii < DIGITS; ii++)
	{
		result->digits[ii] = 0;
		if (all_zeros == true && (a->digits[ii] != 0 || b->digits[ii] != 0))
			all_zeros = false;
	}

	if (all_zeros)
	{
		result->sign = true;
		result->decpos = 0;
		return;
	}

	result->sign = true;
	result->decpos = a->decpos > b->decpos ? a->decpos : b->decpos;


	int pos_diff = a->decpos - b->decpos;
	bool a_on_top = GreaterThan(a, b);
	if (a_on_top == false)
		result->sign = false;

	if (pos_diff < 0)
		pos_diff = -pos_diff;

	for (int ii = DMO; ii >= 0; ii--)
	{
		int bot_idx = ii - pos_diff;
		int sum = (a_on_top ? a->digits[ii] : b->digits[ii]) - (bot_idx < 0 ? 0 : (a_on_top ? b->digits[bot_idx] : a->digits[bot_idx])) - take;
		if (sum < 0)
		{
			take = 1;
			sum += MAGNITUDE;
		}
		else
			take = 0;

		result->digits[ii] = sum;
	}

	if (result->digits[0] == 0 && result->decpos > 0)
	{
		// Shift the array left to cinch up the zero.
		for (int jj = 0; jj < DMO - 1; jj++)
		{
			result->digits[jj] = result->digits[jj + 1];
		}
		result->digits[DMO] = 0;
		result->decpos = result->decpos - 1;
	}
}


__device__ void MultiplyArbitrary(ArbDec *a, ArbDec *b, ArbDec *result, ArbInt *a_int, ArbInt *b_int, ArbHugeInt *huge_scratch, ArbHugeInt *each_line, ArbHugeInt *sum_result)
{

	// OPTIMIZE: If a or b is zero, return zero.
	// OPTIMIZE: If a or b is one, return the other one.

	//struct ArbDec result;
	result->sign = true;
	for (int kk = 0; kk < DIGITS; kk++)
		result->digits[kk] = 0;

	bool a_on_top = GreaterThan(a, b);

	//struct ArbInt a_int;
	a_int->sign = a->sign;

	//struct ArbInt b_int;
	b_int->sign = b->sign;

	// Shift everything RIGHT size-number of digits so we have overflow.
	// This is the precision penalty paid when digits approach the max digits.
	int a_zeroes = -1;
	for (int jj = DMO; jj >= 0; jj--)
	{
		if (a->digits[jj] != 0)
		{
			a_zeroes = DMO - jj;
			break;
		}
	}

	int b_zeroes = -1;
	for (int jj = DMO; jj >= 0; jj--)
	{
		if (b->digits[jj] != 0)
		{
			b_zeroes = DMO - jj;
			break;
		}
	}



	// OPTIMIZED: IF EITHER a and b are zero, just return zero.
	if (a_zeroes == -1 || b_zeroes == -1)
	{
		return;
	}


	// Shift everything right to make it useful for integer math.

	for (int jj = DMO; jj >= 0; jj--)
	{
		if (jj >= a_zeroes)
		{
			a_int->digits[jj] = a->digits[jj - a_zeroes];
		}
		else
		{
			a_int->digits[jj] = 0;
		}

		if (jj >= b_zeroes)
		{
			b_int->digits[jj] = b->digits[jj - b_zeroes];
		}
		else
		{
			b_int->digits[jj] = 0;
		}
	}


	if (a_on_top)
		MultiplyHugePositiveIntegers(a_int, b_int, huge_scratch, each_line, sum_result);
	else
		MultiplyHugePositiveIntegers(b_int, a_int, huge_scratch, each_line, sum_result);

	int size = (DMO - a_zeroes) + (DMO - b_zeroes);

	result->sign = huge_scratch->sign;

	for (int jj = 0; jj <= DMO; jj++)
	{
		int idx_huge = HDMO - size + jj - 1;
		if (idx_huge <= HDMO)
			result->digits[jj] = huge_scratch->digits[idx_huge];
		else
			result->digits[jj] = 0;
	}
	result->decpos = a->decpos + b->decpos;


	// Now trim off leading zeros by shifting left.
	int result_non_zero_idx = -1;
	for (int jj = 0; jj < DMO; jj++)
	{
		if (result->digits[jj] != 0)
		{
			result_non_zero_idx = jj;
			break;
		}
	}

	if (result_non_zero_idx == -1)
	{
		result->decpos = 0;
		result->sign = true;
	}
	else
	{
		if (result_non_zero_idx > result->decpos)
			result_non_zero_idx = result->decpos;

		if (result_non_zero_idx > 0)
		{
			int over = DMO - result_non_zero_idx;
			for (int jj = 0; jj < DMO; jj++)
			{
				result->digits[jj] = jj > over ? 0 : result->digits[jj + result_non_zero_idx];
			}
			result->decpos = result->decpos - result_non_zero_idx;

			if (result->decpos < 0)
				result->decpos = 0;
		}
	}

	bool a_sign = a->sign;
	bool b_sign = b->sign;
	if (a_sign == false && b_sign == false)
		result->sign = true;
	else if (a_sign == false || b_sign == false)
		result->sign = false;
}



__device__ void Add(ArbDec *a, ArbDec *b, ArbDec *result)
{
	// -a + b = b - a
	if (a->sign == false && b->sign == true)
	{
		SubtractTwoPositiveArbitrary(b, a, result);
	}
	// a + -b = a - b
	else if (a->sign == true && b->sign == false)
	{
		SubtractTwoPositiveArbitrary(a, b, result);
	}
	// a + b = a + b
	else if (a->sign == true && b->sign == true)
	{
		AddTwoPositiveArbitrary(a, b, result);
	}
	// -a + -b = -(a+b)
	else if (a->sign == false && b->sign == false)
	{
		AddTwoPositiveArbitrary(a, b, result);
		result->sign = false;
		result;
	}
}



__device__ void Subtract(ArbDec *a, ArbDec *b, ArbDec *result)
{
	// -a - b = -(a+b)
	if (a->sign == false && b->sign == true)
	{
		AddTwoPositiveArbitrary(a, b, result);
		result->sign = false;
	}
	// a - -b = a + b
	else if (a->sign == true && b->sign == false)
	{
		AddTwoPositiveArbitrary(a, b, result);
	}
	// a - b = a - b
	else if (a->sign == true && b->sign == true)
	{
		SubtractTwoPositiveArbitrary(a, b, result);
	}
	// -a - -b = -a + b = b - a
	else if (a->sign == false && b->sign == false)
	{
		SubtractTwoPositiveArbitrary(b, a, result);
	}
}


__device__ bool KeepGoing(ArbDec *x, ArbDec *y, ArbDec *x2, ArbDec *y2, ArbDec *sum, ArbInt *a_int, ArbInt *b_int, ArbHugeInt *huge_scratch, ArbHugeInt *each_line, ArbHugeInt *sum_result)
{
	// x ^ 2
	MultiplyArbitrary(x, x, x2, a_int, b_int, huge_scratch, each_line, sum_result);

	// y ^ 2
	MultiplyArbitrary(y, y, y2, a_int, b_int, huge_scratch, each_line, sum_result);

	// sum the squares
	Add(x2, y2, sum);

	// Keep going if the sum is less than four.
	bool result = sum->decpos == 0 ? true : sum->digits[sum->decpos - 1] < 4;
	return result;
}

__device__ void TimesTwo(ArbDec *a, ArbDec *result, ArbDec *two, ArbInt *a_int, ArbInt *b_int, ArbHugeInt *huge_scratch, ArbHugeInt *each_line, ArbHugeInt *sum_result)
{
	MultiplyArbitrary(a, two, result, a_int, b_int, huge_scratch, each_line, sum_result);
}


__global__ void kernel(double** map, int *c, int xsize,
	int y_scale_sign, int y_scale_decpos, uint16_t *y_scale_digits,
	int  y_base_sign, int y_base_decpos, uint16_t *y_base_digits,
	int  x_scale_sign, int x_scale_decpos, uint16_t *x_scale_digits,
	int  x_base_sign, int x_base_decpos, uint16_t *x_base_digits,
	int max)
{

	int idx_x = blockDim.x * blockIdx.x + threadIdx.x;
	int idx_y = blockDim.y * blockIdx.y + threadIdx.y;
	int idx = xsize * idx_y + idx_x;


	// The number two!
	ArbDec two = GetZeroDec();
	two.sign = true;
	two.decpos = 1;
	two.digits[0] = 2;


	// Indexes.. where are we?
	ArbDec idx_x_arb = ArbFromDouble((double)idx_x);
	ArbDec idx_y_arb = ArbFromDouble((double)idx_y);


	// Contract the bases and scales.
	ArbDec x_base = Construct(x_base_sign, x_base_decpos, x_base_digits);
	ArbDec x_scale = Construct(x_scale_sign, x_scale_decpos, x_scale_digits);
	ArbDec y_base = Construct(y_base_sign, y_base_decpos, y_base_digits);
	ArbDec y_scale = Construct(y_scale_sign, y_scale_decpos, y_scale_digits);


	// Scratchpad vars for small memory footprint.
	// DONT use GetZeroDec here because i want to do a single loop.
	ArbDec x0;
	ArbDec x_mult;
	ArbDec y0;
	ArbDec y_mult;
	ArbDec x;
	ArbDec y;
	ArbInt a_int;
	ArbInt b_int;
	ArbHugeInt huge_scratch;
	ArbHugeInt each_line;
	ArbHugeInt sum_result;
	ArbDec x2;
	ArbDec y2;
	ArbDec sum;
	ArbDec diff;
	ArbDec temp;
	ArbDec x_times_2;
	ArbDec x_times_2_times_y;


	for (int kk = 0; kk < DIGITS; kk++)
	{
		x0.sign = true;
		y0.sign = true;
		x_mult.sign = true;
		y_mult.sign = true;
		x.sign = true;
		y.sign = true;
		a_int.sign = true;
		b_int.sign = true;
		huge_scratch.sign = true;
		each_line.sign = true;
		sum_result.sign = true;
		x2.sign = true;
		y2.sign = true;
		sum.sign = true;
		diff.sign = true;
		temp.sign = true;
		x_times_2.sign = true;
		x_times_2_times_y.sign = true;


		x0.digits[kk] = 0;
		y0.digits[kk] = 0;
		x_mult.digits[kk] = 0;
		y_mult.digits[kk] = 0;
		x.digits[kk] = 0;
		y.digits[kk] = 0;
		a_int.digits[kk] = 0;
		b_int.digits[kk] = 0;
		huge_scratch.digits[kk] = 0;
		each_line.digits[kk] = 0;
		sum_result.digits[kk] = 0;
		x2.digits[kk] = 0;
		y2.digits[kk] = 0;
		sum.digits[kk] = 0;
		diff.digits[kk] = 0;
		temp.digits[kk] = 0;
		x_times_2.digits[kk] = 0;
		x_times_2_times_y.digits[kk] = 0;
	}

	// Calculate x0 from base and scale
	MultiplyArbitrary(&idx_x_arb, &x_scale, &x_mult, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
	Add(&x_base, &x_mult, &x0);

	// Calculate y0 from base and scale.
	MultiplyArbitrary(&idx_y_arb, &y_scale, &y_mult, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
	Add(&y_base, &y_mult, &y0);


	int ii = 0;
	while (KeepGoing(&x, &y, &x2, &y2, &sum, &a_int, &b_int, &huge_scratch, &each_line, &sum_result) && ii < max)
	{
		// Calculate x
		MultiplyArbitrary(&x, &x, &x2, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
		MultiplyArbitrary(&y, &y, &y2, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
		Subtract(&x2, &y2, &diff);
		Add(&diff, &x0, &temp);

		// Calculate y
		TimesTwo(&x, &x_times_2, &two, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
		MultiplyArbitrary(&x_times_2, &y, &x_times_2_times_y, &a_int, &b_int, &huge_scratch, &each_line, &sum_result);
		Add(&x_times_2_times_y, &y0, &y);

		// Move temp into x.
		for (int kk = 0; kk < DIGITS; kk++)
			x.digits[kk] = temp.digits[kk];


		// Increase the iterator!!
		ii++;
	}

	//printf("%d took %d iterations\n", idx, ii);

	c[idx] = ii;
}

int main()
{
	uint32_t j = 0;
	return 0;
}