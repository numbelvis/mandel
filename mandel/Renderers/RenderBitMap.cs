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

        public override void ProcessBlockResult(ILocation location, ushort[] values, int width, int y0, int lines_per)
        {
            throw new NotImplementedException();
        }


        public override Bitmap GetFinalResult()
        {
            throw new NotImplementedException();
        }
    }
}
