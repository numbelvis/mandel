﻿using System;
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
        public static readonly uint Magnitude = Convert.ToUInt32(Math.Pow(10, NumberOfDigitsPer));
    }
}
