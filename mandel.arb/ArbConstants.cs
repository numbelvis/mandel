using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.arb
{
    public static class ArbConstants
    {
        /// <summary>
        /// Number of digits that can be represented in each slot of the array.  A fundamental constant of the entire system.
        /// </summary>
        public const ushort NumberOfDigitsPer = 4;


        /// <summary>
        /// The number of numbers that can be represented by each slot of the array.  We are using base 10.
        /// </summary>
        public static readonly int Magnitude = (int)Math.Pow(10, NumberOfDigitsPer);


        /// <summary>
        /// Size of the digits array.  This number times Number of Digits equals out to the total number of digits of precision the number can represent.
        /// </summary>
        public const int DigitArraySize = 25;

        // TWICE as many digits as DigitArraySize!
        public static readonly ushort HugeDigitArraySize = DigitArraySize * 2;

    }
}
