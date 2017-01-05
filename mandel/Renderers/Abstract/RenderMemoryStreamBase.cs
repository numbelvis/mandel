using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace mandel
{
    /// <summary>
    /// Render a bitmap and get its bytes as an array
    /// </summary>
    public abstract class RenderMemoryStreamBase : RenderBase<RenderMemoryStreamBase, MemoryStream>, IUseImageFormat
    {
        /// <summary>
        /// Format of the image to emit.
        /// </summary>
        public abstract ImageFormat Format
        { get; }

        /// <summary>
        /// The image is rendered as a bitmap, then we convert it on the way out.
        /// </summary>
        RenderBitMap _render;

        public RenderMemoryStreamBase(int output_width, int output_height)
            : base(output_width, output_height)
        { }


        public override void Initialize(int output_width, int output_height)
        {
            _render = new RenderBitMap(output_width, output_height);
            _render.Initialize(output_width, output_height);
        }

        /// <summary>
        /// Passes rendering duties on to the bitmap renderer
        /// </summary>
        /// <param name="values"></param>
        /// <param name="y0"></param>
        /// <param name="lines_per"></param>
        /// <param name="coloring"></param>
        /// <param name="max_iterations"></param>
        public override void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring, int max_iterations)
        {
            _render.ProcessBlockResult(values, y0, lines_per, coloring, max_iterations);
        }

        public override MemoryStream GetFinalResult()
        {
            // Create a stream to save into.
            var stream = new MemoryStream();

            // Get the bitmap.
            var bmp = _render.GetFinalResult();

            // Save it into the stream.
            bmp.Save(stream, this.Format);

            // Ensure we hand back a stream ready to read.
            stream.Position = 0;

            return stream;            
        }
    }
}
