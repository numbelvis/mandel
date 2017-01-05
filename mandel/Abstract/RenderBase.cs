using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Console.WriteLine(String.Format("Rendering {0} x {1} using {2} threads.", this.OutputWidth, this.OutputHeight, thread_count));

            var calculator = Activator.CreateInstance(typeof(Tcalc), new object[] { location, this.OutputWidth, this.OutputHeight }) as CalculatorBase<Tcalc, Tnumbers>;
            var coloring = Activator.CreateInstance(typeof(Tcoloring), new object[] { max_iterations }) as ColoringBase;

            var y = 0;
            var block_height = thread_count * lines_per;
            while (y < this.OutputHeight)
            {
                var results = new ushort[thread_count][];
                var countdown = new CountdownEvent(thread_count);
                for (var tt = 0; tt < thread_count; tt++)
                {
                    var y_0 = y + (tt * lines_per);
                    var tt_for_closure = tt;
                    new Thread(() =>
                    {
                        var lines = calculator.CalculateLines(y_0, lines_per, max_iterations);
                        results[tt_for_closure] = lines;

                        countdown.Signal();
                    }).Start();
                }
                countdown.Wait();

                // Write them sequentially.
                for (var tt = 0; tt < thread_count; tt++)
                {
                    var y_0 = y + (tt * lines_per);
                    var line = results[tt];
                    ProcessBlockResult(line, y_0, lines_per, coloring, max_iterations);
                }

                y += block_height;
            }

            return GetFinalResult();
        }

        #endregion
    }
}
