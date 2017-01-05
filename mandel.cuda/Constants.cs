using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel.cuda
{
    public static class Constants
    {
        // GPU threads per block.
        //public const int GPU_Threads_x = 20;
        //public const int GPU_Threads_y = 20;

        // GPU Blocks per grid
        //public const int GPU_Blocks_x = 3;
        //public const int GPU_Blocks_y = 2;

        // A single GPU is currently supported.
        public const int GPU_DeviceId = 1;
    }
}
