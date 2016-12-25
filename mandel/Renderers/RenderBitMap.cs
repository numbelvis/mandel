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

        public override void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring)
        {
            for (int ii = 0; ii < lines_per; ii++)
            {
                for(int x = 0; x < this.OutputWidth; x++)
                {
                    var color = coloring.Get(values[ii * lines_per + x]);
                    _bitmap.SetPixel(x, y0 + ii, color);
                }
            }
        }

        public override Bitmap GetFinalResult()
        {
            return _bitmap;
        }
    }
}
