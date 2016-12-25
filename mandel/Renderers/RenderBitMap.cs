using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public class RenderBitMap : RenderBase<RenderBitMap, Bitmap>
    {
        Bitmap _bitmap = new Bitmap(1,1);

        public RenderBitMap(int output_width, int output_height)
            : base(output_width, output_height)
        { }

        public override void Initialize(int output_width, int output_height)
        {
            _bitmap = new Bitmap(output_width, output_height);
        }

        public override void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring, int max_iterations)
        {
            for (int yy = 0; yy < lines_per; yy++)
            {
                for(int xx = 0; xx < this.OutputWidth; xx++)
                {
                    var color = coloring.Get(values[yy * this.OutputWidth + xx], max_iterations);
                    var y_final = y0 + yy;
                    if (y_final >= this.OutputHeight)
                        break;

                    _bitmap.SetPixel(xx, y_final, color);
                }
            }
        }

        public override Bitmap GetFinalResult()
        {
            return _bitmap;
        }
    }
}
