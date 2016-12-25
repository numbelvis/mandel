using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel.cuda
{
    public static class Constants
    {
        // GPU values.
        public const int GPU_Threads_x = 10;
        public const int GPU_Threads_y = 10;

        public const int GPU_Blocks_x = 2;
        public const int GPU_Blocks_y = 2;
    }
}
