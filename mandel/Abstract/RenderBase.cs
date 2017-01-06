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

        /// <summary>
        /// Return the final output object for this renderer.  GetFinalResult should use results produced by ProcessBlockResult.  This is the responsibility of the Renderer, not the RenderBase.
        /// </summary>
        /// <returns></returns>
        public abstract Toutput GetFinalResult();

        /// <summary>
        /// Process a block of results for use in GetFinalResult
        /// </summary>
        /// <param name="values"></param>
        /// <param name="y0"></param>
        /// <param name="lines_per"></param>
        /// <param name="coloring"></param>
        /// <param name="max_iterations"></param>
        public abstract void ProcessBlockResult(ushort[] values, int y0, int lines_per, ColoringBase coloring, int max_iterations);

        /// <summary>
        /// Perform any tasks that need to be performed before using the renderer.
        /// </summary>
        /// <param name="output_width"></param>
        /// <param name="output_height"></param>
        public abstract void Initialize(int output_width, int output_height);

        #endregion


        #region Properties

        /// <summary>
        /// Width of the rendered's output, which is always 2 dimensional
        /// </summary>
        public int OutputWidth;

        /// <summary>
        /// Height of the rendered's output, which is always 2 dimensional
        /// </summary>
        public int OutputHeight;

        #endregion


        #region Constructor

        /// <summary>
        /// Output Width and Height are required.
        /// </summary>
        /// <param name="output_width"></param>
        /// <param name="output_height"></param>
        public RenderBase(int output_width, int output_height)
        {
            OutputWidth = output_width;
            OutputHeight = output_height;
            Initialize(output_width, output_height);
        }

        #endregion


        #region Rendering

        /// <summary>
        /// Render the Mandelbrot.
        /// </summary>
        /// <typeparam name="Tcalc"></typeparam>
        /// <typeparam name="Tcoloring"></typeparam>
        /// <typeparam name="Tnumbers"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            return Render<Tcalc, Tcoloring, Tnumbers>(location, 100);
        }

        /// <summary>
        /// Render the Mandelbrot.
        /// </summary>
        /// <typeparam name="Tcalc"></typeparam>
        /// <typeparam name="Tcoloring"></typeparam>
        /// <typeparam name="Tnumbers"></typeparam>
        /// <param name="location"></param>
        /// <param name="max_iterations"></param>
        /// <returns></returns>
        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location, int max_iterations)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            return Render<Tcalc, Tcoloring, Tnumbers>(location, max_iterations, Environment.ProcessorCount, 1);
        }

        /// <summary>
        /// Render the mandelbrot data which is then used by the Renderer to create an output.
        /// </summary>
        /// <typeparam name="Tcalc"></typeparam>
        /// <typeparam name="Tcoloring"></typeparam>
        /// <typeparam name="Tnumbers">The type of the number object used by the Calculator.  Must be a class.</typeparam>
        /// <param name="location"></param>
        /// <param name="max_iterations"></param>
        /// <param name="thread_count"></param>
        /// <param name="lines_per"></param>
        /// <returns></returns>
        public Toutput Render<Tcalc, Tcoloring, Tnumbers>(LocationBase<Tnumbers> location, int max_iterations, int thread_count, int lines_per)
            where Tcalc : CalculatorBase<Tcalc, Tnumbers>
            where Tcoloring : ColoringBase
            where Tnumbers : class
        {
            Console.WriteLine(String.Format("Rendering {0} x {1} using {2} CPU threads.", this.OutputWidth, this.OutputHeight, thread_count));

            // Create an instance of the calculator and the coloring
            var calculator = Activator.CreateInstance(typeof(Tcalc), new object[] { location, this.OutputWidth, this.OutputHeight }) as CalculatorBase<Tcalc, Tnumbers>;

            var coloring = Activator.CreateInstance(typeof(Tcoloring), new object[] { max_iterations }) as ColoringBase;


            // Move thru the y values, treating them as lines, and calculate blocks of lines using the number of threads indicated.
            // The current thread model launches threads and waits for them all to complete before writing to the final output.  This means it is only as fast as the slowest thread in the group of threads

            // Our main y value that moves us along until the Output Height is hit
            var y = 0;

            // The total number of lines that will be calculated for each iteration of y's while loop.
            var block_height = thread_count * lines_per;

            // Start the main rendering loop.
            while (y < this.OutputHeight)
            {
                // A 2 dimensional array that stores the value of each thread's results
                var results = new ushort[thread_count][];

                // Countdown allows us to block until all threads are completed.
                var countdown = new CountdownEvent(thread_count);

                // Loop thread_count number of times and launch a new thread with information to calculate a block of lines.
                for (var tt = 0; tt < thread_count; tt++)
                {
                    // The y0 value for this thread.
                    var y_0 = y + (tt * lines_per);

                    // Store the iterator so the closure can use it.  Very important.
                    var tt_for_closure = tt;

                    // Launch a thread.  When the thread is done calculating, it puts its results into the overall results array, and tells the Countdown that it's done.
                    new Thread(() =>
                    {
                        var lines = calculator.CalculateLines(y_0, lines_per, max_iterations);
                        results[tt_for_closure] = lines;

                        // The last instruction in the thread should always be signalling to the Countdown
                        countdown.Signal();
                    }).Start();
                }

                // Block until all of the threads have signalled their completion.
                countdown.Wait();

                // Process the total results from all threads.
                for (var tt = 0; tt < thread_count; tt++)
                {
                    var y_0 = y + (tt * lines_per);
                    var line = results[tt];
                    ProcessBlockResult(line, y_0, lines_per, coloring, max_iterations);
                }

                // Increase y by the total number of lines calculated this time thru the loop.
                y += block_height;
            }

            return GetFinalResult();
        }

        #endregion
    }
}
