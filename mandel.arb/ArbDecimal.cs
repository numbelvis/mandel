using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.arb
{
    public class ArbDecimal : ArbRealNumber<ArbDecimal>
    {
        #region Explanation

        /*
         *  Arb Decimal represent a number with an integer and a fractional portion
         * 
         * Unlike integers, the digits array is packed left to right, with trailing zeros 
         * and never leading zeros in the array.
         * 
         * An index is used to track the position of the decimal point in the number.
         * 
         * Examples: using 10 digit arrays with 4 digit numbers in each array slot:
         * 
         * 10: 10 0 0 0 0 0 0 0 0 0
         * 
         * 999,999: 0 0 0 0 0 0 0 0 99 9999 
         * 
         * 1,000,000,000,000,008: 0 0 0 0 0 0 1 0 0 8
         * 
         */

        #endregion


        #region Additional Properties

        public int decimal_point = 0;

        #endregion


        #region Constructor Conversions

        public ArbDecimal()
            : this(0)
        { }

        public ArbDecimal(int num)
            : this((decimal)num)
        { }

        public ArbDecimal(decimal num)
        {
            Zero(this);

            // Use an integer version to get the whole part.
            var as_int = (int)num;
            var arb_int = new ArbInteger(as_int);

            // Get the fractional part
            var rest = num - (decimal)as_int;
        
            // Set the sign; zero is positive for our calcs
            this.sign = num >= 0;

            // Get the first zero for indexing
            var zero = GetIndexOfFirstNonZeroLeftToRight(arb_int.digits);
            var non_zero_count = ArbConstants.DigitArraySize - zero;

            // Copy over the whole part.
            for (var ii = 0; ii < non_zero_count; ii++)
            {
                this.digits[ii] = arb_int.digits[ii + zero];
            }

            // Set the decimal index
            this.decimal_point = Convert.ToUInt16(non_zero_count);

            // Capture the fractional part down to 8 * magnitude
            if (rest < 0) rest = -rest;
            for(var ii = 0; ii < 8; ii++)
            {
                rest = rest * ArbConstants.Magnitude;
                var part = (int)rest;
                this.digits[ii + non_zero_count] = Convert.ToUInt16(part);
                rest = rest - (decimal)part;
            }
        }

        #endregion


        #region Overrides

        protected override int DigitSize
        {
            get { return ArbConstants.DigitArraySize; }
            set { }
        }

        public override bool BiggerNoSign_Static(ArbDecimal a, ArbDecimal b)
        {
            // Based on decimal point positions we can make an assumption
            if (a.decimal_point != b.decimal_point)
            {
                return a.decimal_point > b.decimal_point;
            }
            else
            {
                for (var ii = 0; ii < ArbConstants.DigitArraySize; ii++)
                {
                    if (a.digits[ii] != b.digits[ii])
                    {
                        return a.digits[ii] > b.digits[ii];
                    }
                }
            }

            return false;
        }


        protected override ArbDecimal AddTwoPositive_Static(ArbDecimal a, ArbDecimal b)
        {
            // Start a result number
            var arb = new ArbDecimal();

            // It is important to put the larger number "on top" for arbitrary calculations.
            var a_larger = BiggerNoSign_Static(a, b);

            // Set the decimal point
            arb.decimal_point = a.decimal_point > b.decimal_point ? a.decimal_point : b.decimal_point;
            
            // Need to know the decimal point difference for the addition loop.
            var decimal_point_diff = a.decimal_point - b.decimal_point;
            if (decimal_point_diff < 0) decimal_point_diff = -decimal_point_diff;
            
            // Start a carry slot
            var carry = (ushort)0;
            
            // Move thru each array slot and add them like back in grade school:
            //       carry
            //  1389 3938.2929
            // +   0.  81 4429
            //----------------
            //    <the answer>

            // Start at DigitArray Size because we add right to left, starting at the last columns in the arrays.
            for (var ii = ArbConstants.DigitArraySize - 1; ii >= 0; ii--)
            {
                // Index to use for the bottom number
                var bottom_idx = ii - decimal_point_diff;

                // Use twice a ushort because of carry.
                int sum = carry +
                    (a_larger ? a.digits[ii] : b.digits[ii]) +
                    (bottom_idx < 0 ? 0 : (a_larger ? b.digits[bottom_idx] : a.digits[bottom_idx]));

                if(sum >= ArbConstants.Magnitude)
                {
                    sum = sum - ArbConstants.Magnitude;
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }

                arb.digits[ii] = Convert.ToUInt16(sum);
            }

            // If there are leftovers, shift and lose some fractional information.  
            if (carry > 0)
            {
                for(var ii = ArbConstants.DigitArraySize - 1; ii > 0; ii--)
                {
                    arb.digits[ii] = arb.digits[ii - 1];
                }
                arb.digits[0] = carry;
                arb.decimal_point = arb.decimal_point + 1;
            }

            return arb;
        }

        protected override ArbDecimal SubtractTwoPositive_Static(ArbDecimal a, ArbDecimal b)
        {
            // Start a result number
            var arb = new ArbDecimal();

            // It is important to put the larger number "on top" for arbitrary calculations.
            var a_larger = BiggerNoSign_Static(a, b);

            // Take slot
            uint take = 0;

            // Set the sign and decimal point
            arb.sign = a_larger;
            arb.decimal_point = a.decimal_point > b.decimal_point ? a.decimal_point : b.decimal_point;

            // decimal point difference for calcs
            var decimal_point_diff = a.decimal_point - b.decimal_point;
            if (decimal_point_diff < 0)
                decimal_point_diff = -decimal_point_diff;

            // Loop thru and subtract like in school
            for(var ii = ArbConstants.DigitArraySize - 1; ii >= 0; ii--)
            {
                var bottom_index = ii - decimal_point_diff;
                int diff = Convert.ToInt32((a_larger ? a.digits[ii] : b.digits[ii]) - (bottom_index < 0 ? 0 : (a_larger ? b.digits[bottom_index] : a.digits[bottom_index])) - take);
                
                if(diff < 0)
                {
                    take = 1;
                    diff = diff + ArbConstants.Magnitude;
                }
                else
                {
                    take = 0;
                }

                arb.digits[ii] = Convert.ToUInt16(diff);
            }

            // If a leading zero presents itself, shift everything to compensate
            if(arb.digits[0] == 0 && arb.decimal_point > 0)
            {
                for(var ii = 0; ii < ArbConstants.DigitArraySize - 2; ii++)
                {
                    arb.digits[ii] = arb.digits[ii + 1];
                }
                arb.digits[ArbConstants.DigitArraySize - 1] = 0;
                arb.decimal_point = arb.decimal_point - 1;
            }

            return arb;
        }

        protected override ArbDecimal MultiplyTwoPositive_Static(ArbDecimal a, ArbDecimal b)
        {
            // Start a result
            var arb = new ArbDecimal();
            arb.sign = true;

            // It is important to put the larger number "on top" for arbitrary calculations.
            var a_larger = BiggerNoSign_Static(a, b);


            // Decimal math is the same as integer math, but then adjust the decimal point.
            // We represent each decimal as an integer, then shift them to line up the decimal points
            // Multiply as integers, then adjust the decimal point appropriately.

            // Represent as integers
            var a_int = new ArbInteger();
            var b_int = new ArbInteger();

            
            // Move the decimal numbers into the int, and pack them to the right.
            var a_zeros = ArbConstants.DigitArraySize - GetIndexOfFirstNonZeroRightToLeft(a.digits) - 1;
            var b_zeros = ArbConstants.DigitArraySize - GetIndexOfFirstNonZeroRightToLeft(b.digits) - 1;
            for (var ii = ArbConstants.DigitArraySize - 1; ii >= 0; ii--)
            {
                a_int.digits[ii] = Convert.ToUInt16(ii >= a_zeros ? a.digits[ii - a_zeros] : 0);
                b_int.digits[ii] = Convert.ToUInt16(ii >= b_zeros ? b.digits[ii - b_zeros] : 0);
            }

            // Now multiply them.
            var mult = new ArbHugeInteger().MultiplyTwoArbIntIntoHuge(a_int, b_int);

            // Set the new decimal point.
            arb.decimal_point = (ushort)(a.decimal_point + b.decimal_point);
            arb.sign = mult.sign;

            var size = (ArbConstants.DigitArraySize - 1 - a_zeros) + (ArbConstants.DigitArraySize - 1 - b_zeros);
            for(var ii = 0; ii < ArbConstants.DigitArraySize - 1; ii++)
            {
                var idx_huge = ArbConstants.HugeDigitArraySize - size + ii - 2;
                if (idx_huge <= ArbConstants.HugeDigitArraySize - 1)
                    arb.digits[ii] = mult.digits[idx_huge];
                else
                    arb.digits[ii] = 0;
            }

            // Reassign the digits back to the decimal number
            var mult_zero_idx = GetIndexOfFirstNonZeroLeftToRight(arb.digits);
            if (mult_zero_idx > arb.decimal_point)
                mult_zero_idx = arb.decimal_point;

            if(mult_zero_idx > 0)
            {
                var over = ArbConstants.DigitArraySize - mult_zero_idx - 1;
                for(var ii = 0; ii < ArbConstants.DigitArraySize - 1; ii++)
                {
                    arb.digits[ii] = Convert.ToUInt16(ii > over ? 0 : arb.digits[ii + mult_zero_idx]);
                }
                arb.decimal_point = arb.decimal_point - mult_zero_idx;

                if(arb.decimal_point < 0)
                    arb.decimal_point = 0;
            }

            return arb;
        }

        #endregion
    }
}
