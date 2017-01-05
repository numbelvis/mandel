using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.arb
{
    public abstract class ArbRealNumber<T>
        where T : ArbRealNumber<T>, new()
    {
        #region Abstracts

        protected abstract T AddTwoPositive_Static(T a, T b);

        protected abstract T SubtractTwoPositive_Static(T a, T b);

        protected abstract T MultiplyTwoPositive_Static(T a, T b);

        public abstract bool BiggerNoSign_Static(T a, T b);

        protected abstract int DigitSize
        { get; set; }
        
        #endregion


        #region Properties

        // Sign
        public bool sign;

        // Digits array
        public ushort[] digits;

        // Indicates this number had an overflow.
        public bool overflow;

        #endregion


        #region Constructors

        public ArbRealNumber()
        {
            Zero(this);
        }

        #endregion


        #region Zero

        protected static void Zero(ArbRealNumber<T> val)
        {
            val.digits = new ushort[val.DigitSize];
            val.sign = true;
            for (var ii = 0; ii < val.DigitSize; ii++)
                val.digits[ii] = 0;
        }

        /// <summary>
        /// Check if this number is zero, and set sign accordingly.
        /// </summary>
        public void CheckForZero()
        {
            for (var ii = 0; ii < DigitSize; ii++)
                if (digits[ii] != 0)
                    return;

            sign = true;
        }

        #endregion


        #region Operators

        /// <summary>
        /// Adds a and b, returns the sum.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator +(ArbRealNumber<T> a, ArbRealNumber<T> b)
        {
            // There's a long winded reason why I do this
            var math = GetStatic();

            // Start a result.
            var result = (T)null;

            // Adjust the operation based on signs of a and b.

            // (a + b)
            if (a.sign && b.sign)
            {
                result = math.AddTwoPositive_Static((T)a, (T)b);
            }

            // (-a + b) becomes (b - a)
            else if (a.sign == false && b.sign)
            {
                result = math.SubtractTwoPositive_Static((T)b, (T)a);
            }

            // (a + -b) becomes (a - b)
            else if (a.sign && b.sign == false)
            {
                result = math.SubtractTwoPositive_Static((T)a, (T)b);
            }

            // -a + -b becomes -(a + b)
            else if (a.sign == false && b.sign == false)
            {
                result = math.AddTwoPositive_Static((T)a, (T)b);
                result.sign = true;
            }

            result.CheckForZero();
            return result;
        }


        /// <summary>
        /// Subtracts b from a and returns the difference.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator -(ArbRealNumber<T> a, ArbRealNumber<T> b)
        {
            // A neutral object used to do the maths.
            var math = GetStatic();

            // Start a result.
            var result = (T)null;
            
            // Adjust the operation based on signs of a and b.

            // (a - b)
            if (a.sign && b.sign)
            {
                result = math.SubtractTwoPositive_Static((T)a, (T)b);
            }

            // (-a - b) becomes -(a + b)
            else if (a.sign == false && b.sign)
            {
                result = math.AddTwoPositive_Static((T)a, (T)b);
                result.sign = false;
            }

            // (a - -b) becomes (a + b)
            else if (a.sign && b.sign == false)
            {
                result = math.AddTwoPositive_Static((T)a, (T)b);
            }

            // -a - -b becomes b - a
            else if (a.sign == false && b.sign == false)
            {
                result = math.SubtractTwoPositive_Static((T)b, (T)a);
            }

            result.CheckForZero();
            return result;
        }

        /// <summary>
        /// Multiply a and b and return the result.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T operator *(ArbRealNumber<T> a, ArbRealNumber<T> b)
        {
            // A neutral object used to do the maths.
            var math = GetStatic();

            // Calculate the sign immediately; it's unaffected by the numeric calcs
            var sign = true;

            // a* b = + and -a * -b = +
            if ((a.sign && b.sign) || (a.sign == false && b.sign == false))
            {
                sign = true;
            }

            // a * -b = - and -a * b = -
            else if((a.sign && b.sign == false) || a.sign == false && b.sign)
            {
                sign = false;
            }

            // Do the math, attach the sign, send it on its way
            var result = math.MultiplyTwoPositive_Static((T)a, (T)b);
            result.sign = sign;
            result.CheckForZero();
            return result;
        }


        #endregion


        #region Private

        public static T GetStatic()
        {
            return Activator.CreateInstance(typeof(T), new object[] { 0 }) as T;
        }


        public static int GetIndexOfFirstNonZeroLeftToRight(ushort[] array)
        {
            for (var ii = 0; ii < array.Length; ii++)
            {
                if (array[ii] != 0)
                    return ii;
            }

            return array.Length;
        }

        public static int GetIndexOfFirstNonZeroRightToLeft(ushort[] array)
        {
            for (var ii = array.Length - 1; ii >= 0; ii--)
            {
                if (array[ii] != 0)
                    return ii;
            }

            return 0;
        }

        #endregion
    }
}
