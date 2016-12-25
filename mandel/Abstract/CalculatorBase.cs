using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace mandel
{
    public abstract class CalculatorBase<T, Tnumber>
        where T : class
        where Tnumber : class
    {
        #region Abstracts

        public abstract int GetWidthDivisionCount();

        public abstract int GetWidthDivisionSize();

        public abstract ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations);

        #endregion


        #region Public

        public LocationBase<Tnumber> Location;
        public int OutputWidth;
        public int OutputHeight;
        public int WidthDivisions;
        public int EachWidthDivision;
        public Tnumber ColumnWidth;
        public Tnumber LineHeight;

        public CalculatorBase(LocationBase<Tnumber> location, int output_width, int output_height)
        {
            this.Location = location;
            this.OutputWidth = output_width;
            this.OutputHeight = output_height;

            this.LineHeight = location.CalculateLineHeight(output_height);
            this.ColumnWidth = location.CalculateColumnWidth(output_width);

            this.WidthDivisions = GetWidthDivisionCount();
            this.EachWidthDivision = GetWidthDivisionSize();
        }

        public ushort[] CalculateLines(int y_start, int lines_per, int max_iterations)
        {
            var result = new ushort[this.OutputWidth * lines_per];

            for (var x = 0; x < this.WidthDivisions; x++)
            {
                var iterations = DoBlock(x, this.EachWidthDivision, y_start, lines_per, max_iterations);
                
                // Copy the results of this block onto the final results.
                for(var ii = 0; ii < lines_per; ii++)
                {
                    for(var jj = 0; jj < this.EachWidthDivision; jj++)
                    {
                        result[(ii * this.OutputWidth) + (x * this.EachWidthDivision) + jj] = iterations[ii * this.EachWidthDivision + jj];
                    }
                }
            }

            return result;
        }
        
        #endregion


        #region Private



        #endregion
    }
}
