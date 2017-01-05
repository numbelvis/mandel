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
    public abstract class RenderImageBytesBase<T> : RenderBase<RenderImageBytesBase<T>, byte[]>
        where T : RenderMemoryStreamBase
    {
        /// <summary>
        /// Use the memory stream render.
        /// </summary>
        T _render;

        public RenderImageBytesBase(int output_width, int output_height)
            : base(output_width, output_height)
        { }


        public override void Initialize(int output_width, int output_height)
        {
            _render = Activator.CreateInstance(typeof(T),  new object[] { output_width, output_height }) as T;
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

        public override byte[] GetFinalResult()
        {
            // Get the stream
            var stream = _render.GetFinalResult();

            // Return the bytes in the stream
            return stream.ToArray();
        }
    }
}
