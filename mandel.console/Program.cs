using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using mandel;
using mandel.cuda;

namespace mandel.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var bitmap = new RenderBitMap(800, 600)
                                .Render<RegularMathCalculator, WaveyColoring, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 100, 1,1);

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "examples");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var filename = Path.Combine(path, "fractal.bmp");
            bitmap.Save(filename);
            /*
            var cuda_bitmap = new RenderBitMap(800, 600).Render<CudaRegularMathCalculator, WaveyColoring, MDecimal>(new Location()
            {
                x0 = -2.5m,
                xMax = 1m,
                y0 = -1m,
                yMax = 1m
            });
             */
        }
    }
}
