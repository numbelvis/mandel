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

        public abstract void ProcessBlockResult(ILocation location, ushort[] values, int width, int y0, int lines_per);

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

        public Toutput Render<C>(ILocation location)
            where C : CalculatorBase<C>, new()
        {
            return Render<C>(location, 100);
        }

        public Toutput Render<C>(ILocation location, int max_iterations)
                    where C : CalculatorBase<C>, new()
        {
            return Render<C>(location, max_iterations, Environment.ProcessorCount, 1);
        }


        public Toutput Render<C>(ILocation location, int max_iterations, int thread_count, int lines_per)
            where C : CalculatorBase<C>, new()
        {
            var calculator = Activator.CreateInstance<C>();

            var y = 0;
            var block_height = thread_count * lines_per;
            while (y < this.OutputHeight)
            {
                // A block is broken up into threads.
                var tasks = new List<Task<ushort[]>>();
                for(var thread_num = 0; thread_num < thread_count; thread_num++)
                {
                    var y_0 = y + (thread_num * lines_per);
                    tasks.Add(new Task<ushort[]>(() => calculator.CalculateBlock(location, y_0, lines_per, max_iterations)));
                }

                // meter threading by waiting until they are all complete.
                Task.WaitAll(tasks.ToArray());

                // Render each block.
                foreach(var task in tasks)
                {
                    
                }

                y += block_height;
            }

            return null;
        }

        #endregion
    }
}
