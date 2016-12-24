using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.arb
{
    public abstract class ArbRealNumber<T>
        where T : class, new()
    {
        #region Abstracts

        // Sign
        protected bool sign;

        // Digits array
        protected uint[] digits;
        
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
            val.sign = true;
            for (var ii = 0; ii < ArbConstants.NumberOfDigitsPer; ii++)
                val.digits[ii] = 0;
        }

        #endregion

        #region Operators

        public static T operator +(ArbRealNumber<T> a, ArbRealNumber<T> b)
        {
            return null;
        }

        #endregion
    }
}
