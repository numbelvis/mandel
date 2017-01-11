using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace mandel
{
    public class RenderJpegBytes : RenderImageBytesBase<RenderJpegMemoryStream>
    {
        public RenderJpegBytes(int output_width, int output_height)
            : base(output_width, output_height)
        { }
    }
}
