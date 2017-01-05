using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel.arb
{
    public class ArbInteger : ArbRealNumber<ArbInteger>
    {
        #region Explanation

        /*
         *  Arb Integers represent a whole number only.
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

        public ArbInteger()
            : this(0)
        { }

        public ArbInteger(int num)
        {
            Zero(this);

            // Set the sign immediately.  True is positive.
            this.sign = num >= 0;

            if (num < 0)
                num = -num;

            for(var ii = ArbConstants.DigitArraySize - 1; ii >= 0; ii--)
            {
                int this_digit = Convert.ToInt32(num % ArbConstants.Magnitude);
                this.digits[ii] = Convert.ToUInt16(this_digit);
                num = Convert.ToInt32(num / ArbConstants.Magnitude);
            }
        }

        public ArbInteger(decimal num)
            : this((int)num)
        { }

        #endregion


        #region Overrides

        protected override int DigitSize
        {
            get { return ArbConstants.DigitArraySize; }
            set { }
        }

        public override bool BiggerNoSign_Static(ArbInteger a, ArbInteger b)
        {
            for (var ii = 0; ii < ArbConstants.DigitArraySize; ii++)
                if (a.digits[ii] != b.digits[ii])
                    return a.digits[ii] > b.digits[ii];

            return false;
        }


        protected override ArbInteger AddTwoPositive_Static(ArbInteger a, ArbInteger b)
        {
            // Start a result number
            var arb = new ArbInteger();

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

            // Start at DigitArraySize because we add right to left, starting at the last columns in the arrays.
            for (var ii = ArbConstants.DigitArraySize - 1; ii >= 0; ii--)
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

            // If there are leftovers, then we lost info.
            if (carry == 1)
                throw new ArbAdditionOverflow();

            return arb;
        }

        protected override ArbInteger SubtractTwoPositive_Static(ArbInteger a, ArbInteger b)
        {
            throw new NotImplementedException();
        }

        protected override ArbInteger MultiplyTwoPositive_Static(ArbInteger a, ArbInteger b)
        {
            // Do the multiply.
            var huge_result = new ArbHugeInteger().MultiplyTwoArbIntIntoHuge(a, b);
            
            // Move the results back into an integer.
            var arb = new ArbInteger();
            for (var ii = ArbConstants.HugeDigitArraySize - 1; ii >= ArbConstants.DigitArraySize; ii--)
            {
                arb.digits[ii - ArbConstants.DigitArraySize] = huge_result.digits[ii];
            }

            return arb;
        }

        #endregion
    }
}
