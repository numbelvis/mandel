using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using mandel;
using mandel.cuda;
using mandel.arb;

namespace mandel.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "examples");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);


            /* Rendering bytes instead of the bitmap object. 
            var bitmap_bytes = new RenderBmpBytes(400, 300)
                                .Render<RegularMathCalculator, WaveyColoring, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 250, 8, 20);

            var filename = Path.Combine(path, "fractal.bmp");
            File.WriteAllBytes(filename, bitmap_bytes);
            */

            /* Rendering png bytes instead of the bitmap object.
            var png_bytes = new RenderPngBytes(400, 300)
                                .Render<RegularMathCalculator, WaveyColoring, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 250, 8, 20);

            var filename = Path.Combine(path, "fractal.png");
            File.WriteAllBytes(filename, png_bytes);
            */

            /* Rendering jpeg bytes instead of the bitmap object. */
            var jpeg_bytes = new RenderPngBytes(1920, 1200)
                                .Render<CudaRegularMathCalculator, SingleColor, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 1000, 8, 20);

            var filename = Path.Combine(path, "fractal.jpg");
            File.WriteAllBytes(filename, jpeg_bytes);



            
            /*
                        var bitmap = new RenderBitMap(800, 600)
                                            .Render<RegularMathCalculator, WaveyColoring, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 250, 8, 20);
                        */

            /*
            var bitmap = new RenderBitMap(800, 600)
                                            .Render<RegularMathCalculator, SingleColor, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 250, 8, 20);
            */


            /*
            var bitmap = new RenderBitMap(800, 600)
                                .Render<CudaRegularMathCalculator, WaveyColoring, MDecimal>(new Location(-2.5m, 1m, -1m, 1m), 250, 1, 20);

            */
            /*
            var bitmap = new RenderBitMap(400, 300)
                                            .Render<ArbitraryPrecisionMathCalculator, WaveyColoring, ArbDecimal>(new ArbLocation(new ArbDecimal(-2.5m), 
                                                new ArbDecimal(1m),
                                                new ArbDecimal(-1m),
                                                new ArbDecimal(1m)), 250, 8, 5);
            */

            /*
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "examples");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var filename = Path.Combine(path, "fractal.bmp");
            bitmap.Save(filename);
             */
        }
    }
}
