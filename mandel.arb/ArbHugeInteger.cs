using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel.arb
{
    public class ArbHugeInteger : ArbRealNumber<ArbHugeInteger>
    {
        #region Explanation

        /*
         *  Arb Huge Integers represent a whole number with twice as many digits as an Arb Integer.
         * 
         * The digits array is packed right to left with leading zeros to the left.
         * 
         * Examples: using 10 digit arrays with 4 digit numbers in each array slot:
         * 
         * 10: 0 0 0 0 0 0 0 0 0 10
         * 
         * 999,999: 0 0 0 0 0 0 0 0 99 9999 
         * 
         * 1,000,000,000,000,008: 0 0 0 0 0 0 1 0 0 8
         * 
         */

        #endregion


        #region Constructor Conversions

        public ArbHugeInteger()
            : this(0)
        { }

        public ArbHugeInteger(int num)
        {
            Zero(this);

            // Set the sign immediately.  True is positive.
            this.sign = num >= 0;

            if (num < 0)
                num = -num;

            for (var ii = ArbConstants.HugeDigitArraySize - 1; ii >= 0; ii--)
            {
                int this_digit = Convert.ToInt32(num % ArbConstants.Magnitude);
                this.digits[ii] = Convert.ToUInt16(this_digit);
                num = Convert.ToInt32(num / ArbConstants.Magnitude);
            }
        }

        public ArbHugeInteger(decimal num)
            : this((int)num)
        { }

        #endregion


        #region Overrides

        protected override int DigitSize
        {
            get { return ArbConstants.HugeDigitArraySize; }
            set { }
        }

        public override bool BiggerNoSign_Static(ArbHugeInteger a, ArbHugeInteger b)
        {
            for (var ii = 0; ii < ArbConstants.HugeDigitArraySize; ii++)
                if (a.digits[ii] != b.digits[ii])
                    return a.digits[ii] > b.digits[ii];

            return false;
        }

        protected override ArbHugeInteger AddTwoPositive_Static(ArbHugeInteger a, ArbHugeInteger b)
        {
            // Start a result number
            var arb = new ArbHugeInteger();

            // It is important to put the larger number "on top" for arbitrary calculations.
            var a_larger = BiggerNoSign_Static(a, b);

            // Start a carry slot
            var carry = (ushort)0;
            
            // Move thru each array slot and add them like back in grade school:
            //       carry
            //  1389 3938 2929
            // +   0   81 4429
            //----------------
            //    <the answer>

            // Start at HugeDigitArraySize because we add right to left, starting at the last columns in the arrays.
            for (var ii = ArbConstants.HugeDigitArraySize - 1; ii >= 0; ii--)
            {
                // Use twice a ushort because of carry.
                int sum = a.digits[ii] + b.digits[ii] + carry;
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

            return arb;
        }

        protected override ArbHugeInteger SubtractTwoPositive_Static(ArbHugeInteger a, ArbHugeInteger b)
        {
            throw new NotImplementedException();
        }

        protected override ArbHugeInteger MultiplyTwoPositive_Static(ArbHugeInteger a, ArbHugeInteger b)
        {
            throw new NotImplementedException();
        }

        public ArbHugeInteger MultiplyTwoArbIntIntoHuge(ArbInteger a, ArbInteger b)
        {
            // Start a result number
            var arb = new ArbHugeInteger();

            // It is important to put the larger number "on top" for arbitrary calculations.
            var a_larger = new ArbInteger().BiggerNoSign_Static(a, b);

            // Start a carry slot
            var carry = (int)0;

            // Move thru each array slot and add them like back in grade school:
            //       carry
            //  1389 3938 2929
            //  x       81 4429
            //  ---------------
            //        1234 1234
            // + 1234 1234    0
            //  ---------------
            //    <the answer>


            // Save some time by finding the "size" of the numbers and only multiplying those numbers.  This avoids all lots of zero * zero empty calculations for smaller numbers.
            var a_size = GetIndexOfFirstNonZeroLeftToRight(a.digits) - 1;
            var b_size = GetIndexOfFirstNonZeroLeftToRight(b.digits) - 1;
            var top_size = a_larger ? a_size : b_size;
            var bottom_size = a_larger ? b_size : a_size;

            // Start multiplying each slot and then add the results!
            for (var ii = ArbConstants.DigitArraySize - 1; ii > bottom_size; ii--)
            {
                // Shift it over every slot we move
                var idx_move = ArbConstants.DigitArraySize - ii - 1;

                var line_result = new ArbHugeInteger(0);

                for (var jj = ArbConstants.DigitArraySize - 1; jj > top_size; jj--)
                {
                    int prod = (a_larger ? a.digits[jj] * b.digits[ii] : a.digits[ii] * b.digits[jj]) + carry;
                    if(prod >= ArbConstants.Magnitude)
                    {
                        carry = Convert.ToInt32(prod / ArbConstants.Magnitude);
                        prod = prod - Convert.ToInt32(carry * ArbConstants.Magnitude);
                    }
                    else
                    {
                        carry = 0;
                    }

                    var idx = jj - idx_move + ArbConstants.DigitArraySize;
                    line_result.digits[idx] = Convert.ToUInt16(prod);
                }

                if(carry > 0)
                {
                    var idx = top_size - idx_move + ArbConstants.DigitArraySize;
                    line_result.digits[idx] = Convert.ToUInt16(carry);
                }

                arb = AddTwoPositive_Static(arb, line_result);
            }

            // Set the final sign based on the a and b's signs
            arb.sign = !(a.sign ^ b.sign);
            arb.CheckForZero();
            return arb;
        }

        #endregion
    }
}
