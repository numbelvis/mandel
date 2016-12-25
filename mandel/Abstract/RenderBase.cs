using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel
{
    public abstract class RenderBase<T, Toutput>
        where Toutput : class
    {
        #region Abstracts

        public abstract Toutput GetFinalResult();

        public abstract void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring, int max_iterations);

        public abstract void Initialize(int output_width, int output_height);

        #endregion


        #region Properties

        public int OutputWidth;

        public int OutputHeight;

        #endregion


        #region Constructor

        public RenderBase(int output_width, int output_height)
        {
            OutputWidth = output_width;
            OutputHeight = output_height;
            Initialize(output_width, output_height);
        }

        #endregion


        #region Rendering

        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            return Render<Tcalc, Tcoloring, Tnumbers>(location, 100);
        }

        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location, int max_iterations)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            return Render<Tcalc, Tcoloring, Tnumbers>(location, max_iterations, Environment.ProcessorCount, 1);
        }


        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location, int max_iterations, int thread_count, int lines_per)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            var calculator = Activator.CreateInstance(typeof(Tcalc), new object[] { location, this.OutputWidth, this.OutputHeight }) as CalculatorBase<Tcalc, Tnumbers>;
            var coloring = Activator.CreateInstance(typeof(Tcoloring), new object[] { max_iterations }) as ColoringBase;

            var y = 0;
            var block_height = thread_count * lines_per;
            while (y < this.OutputHeight)
            {
                // A block is broken up into threads.
                var tasks = new List<Task<ushort[]>>();
                for(var thread_num = 0; thread_num < thread_count; thread_num++)
                {
                    var y_0 = y + (thread_num * lines_per);
                    tasks.Add(Task<ushort[]>.Factory.StartNew(() => calculator.CalculateLines(y_0, lines_per, max_iterations)));
                }

                // meter threading by waiting until they are all complete.
                Task.WaitAll(tasks.ToArray());

                // Process each block.
                var count = 0;
                foreach(var task in tasks)
                {
                    var y_0 = y + (count * lines_per);
                    ProcessBlockResult(task.Result, y_0, lines_per, coloring, max_iterations);
                    count++;
                }

                y += block_height;
            }

            return GetFinalResult();
        }

        #endregion
    }
}
