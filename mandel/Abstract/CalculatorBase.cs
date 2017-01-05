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
        public int FinalWidthDivision;
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

            // Final Width Division is used to accomodate output width that doesn't match up nicely with the Calculators Division Widths.  
            this.FinalWidthDivision = this.OutputWidth - (this.WidthDivisions * this.EachWidthDivision);
            if (this.FinalWidthDivision == 0)
                this.FinalWidthDivision = this.EachWidthDivision;
            else if (this.FinalWidthDivision < 0)
                this.FinalWidthDivision = this.EachWidthDivision + this.FinalWidthDivision;
        }


        public ushort[] CalculateLines(int y_start, int lines_count, int max_iterations)
        {
            Console.WriteLine("Calculating lines: {0} - {1}", y_start, y_start + lines_count - 1);

            // Output array is the size of the lines that will be calculated times the output width.
            var result = new ushort[this.OutputWidth * lines_count];

            // Calculate each sub-division in serial.
            for (var division = 0; division < this.WidthDivisions; division++)
            {
                // Get a block for this division for the lines we are calculating.
                var use_each = division == this.WidthDivisions - 1 ? this.FinalWidthDivision : this.EachWidthDivision;
                var iterations = DoBlock(division * use_each, use_each, y_start, lines_count, max_iterations);
                
                // Copy the results of this block onto the final results.
                for (var yy = 0; yy < lines_count; yy++)
                {
                    for (var xx = 0; xx < use_each; xx++)
                    {
                        result[(yy * this.OutputWidth) + (division * use_each) + xx] = iterations[yy * use_each + xx];
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
