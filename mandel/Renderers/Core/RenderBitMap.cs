using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    /// <summary>
    /// Render a .NET Bitmap object full of mandelbrot
    /// </summary>
    public class RenderBitMap : RenderBase<RenderBitMap, Bitmap>
    {
        // The resulting bitmap.
        Bitmap _bitmap = new Bitmap(1,1);

        public RenderBitMap(int output_width, int output_height)
            : base(output_width, output_height)
        { }

        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="output_width"></param>
        /// <param name="output_height"></param>
        public override void Initialize(int output_width, int output_height)
        {
            // Initialize out bitmap object to the output size.
            _bitmap = new Bitmap(output_width, output_height);
        }

        public override void Destroy()
        {
        }

        /// <summary>
        /// Process a block of lines into the bitmap.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="y0"></param>
        /// <param name="lines_per"></param>
        /// <param name="coloring"></param>
        /// <param name="max_iterations"></param>
        public override void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring, int max_iterations)
        {
            // Move thru each line
            for (int yy = 0; yy < lines_per; yy++)
            {
                // And each column in the line
                for(int xx = 0; xx < this.OutputWidth; xx++)
                {
                    // Run the point's result thru the coloring
                    var color = coloring.Get(values[yy * this.OutputWidth + xx], max_iterations);

                    // Calculate where this belongs on the bitmap.
                    var y_final = y0 + yy;

                    // Failsafe in case something weird, it won't bomb.
                    if (y_final >= this.OutputHeight)
                        break;

                    // Finally set the pixel value on our bitmap.
                    _bitmap.SetPixel(xx, y_final, color);
                }
            }
        }

        /// <summary>
        /// Get the final bitmap result.
        /// </summary>
        /// <returns></returns>
        public override Bitmap GetFinalResult()
        {
            // Easy, just return the bitmap.
            return _bitmap;
        }
    }
}
