using System;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace mandel
{
    public class RenderBitmapMemoryStream : RenderMemoryStreamBase
    {
        public RenderBitmapMemoryStream(int output_width, int output_height)
            : base(output_width, output_height)
        { }
        
        public override ImageFormat Format
        {
            get { return ImageFormat.Bmp; }
        }
    }
}
