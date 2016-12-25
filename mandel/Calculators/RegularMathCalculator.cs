using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel
{
    public class RegularMathCalculator : CalculatorBase<RegularMathCalculator, MDecimal>
    {
        public RegularMathCalculator(LocationBase<MDecimal> location, int output_width, int output_height)
            : base(location, output_width, output_height)
        { }

        public override int GetWidthDivisionCount()
        {
            return 1;
        }

        public override int GetWidthDivisionSize()
        {
            return this.OutputWidth;
        }

        public override ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations)
        {
            var result = new ushort[x_count * y_count];

            MDecimal x0 = null;
            MDecimal y0 = null;
            for(var y = 0; y < y_count; y++)
            {
                for(var x = 0; x < x_count; x++)
                {
                    this.Location.EmitPoints(out x0, out y0, x + x_start, y + y_start, this.ColumnWidth, this.LineHeight);
                    result[y * y_count + x] = CalculatePixel(x0.value, y0.value, max_iterations);
                }
            }

            return result;
        }

        ushort CalculatePixel(decimal x0, decimal y0, int max_iterations)
        {
            var x = 0m;
            var y = 0m;
            ushort ii = 0;
            while (x * x + y * y < 4 && ii < max_iterations)
            {
                var temp = x * x - y * y + x0;
                y = 2m * x * y + y0;
                x = temp;
                ii++;
            }
            return ii;
        }
    }
}
