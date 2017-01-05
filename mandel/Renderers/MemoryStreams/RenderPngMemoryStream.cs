using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace mandel
{
    public class RenderPngMemoryStream : RenderMemoryStreamBase
    {
        public RenderPngMemoryStream(int output_width, int output_height)
            : base(output_width, output_height)
        { }
        
        public override ImageFormat Format
        {
            get { return ImageFormat.Png; }
        }
    }
}
