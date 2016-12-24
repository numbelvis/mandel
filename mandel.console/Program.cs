using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using mandel;
using mandel.cuda;
namespace mandel.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var bitmap = new RenderBitMap(800, 600).Render<RegularMathCalculator>(new Location()
                {
                    x0 = -2.5,
                    xMax = 1,
                    y0 = -1,
                    yMax = 1
                });

            var cuda_bitmap = new RenderBitMap(800, 600).Render<CudaRegularMathCalculator>(new Location()
            {
                x0 = -2.5,
                xMax = 1,
                y0 = -1,
                yMax = 1
            });
        }
    }
}
