using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mandel;
using mandel.arb;

namespace mandel
{
    public class ArbitraryPrecisionMathCalculator : CalculatorBase<ArbitraryPrecisionMathCalculator, ArbDecimal>
    {
        public ArbitraryPrecisionMathCalculator(LocationBase<ArbDecimal> location, int output_width, int output_height)
            : base(location, output_width, output_height)
        { }

        public override int GetWidthDivisionCount()
        {
            return 1;
        }

        public override int GetWidthDivisionSize()
        {
            return this.OutputWidth / GetWidthDivisionCount();
        }

        public override void Destroy()
        {
        }

        public override ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations)
        {
            var result = new ushort[x_count * y_count];

            ArbDecimal x0 = null;
            ArbDecimal y0 = null;
            for(var y = 0; y < y_count; y++)
            {
                for(var x = 0; x < x_count; x++)
                {
                    this.Location.EmitPoints(out x0, out y0, x + x_start, y + y_start, this.ColumnWidth, this.LineHeight);
                    result[y * x_count + x] = CalculatePixel(x0, y0, max_iterations);
                }
            }

            return result;
        }

        ushort CalculatePixel(ArbDecimal x0, ArbDecimal y0, int max_iterations)
        {
            var x = new ArbDecimal(0);
            var y = new ArbDecimal(0);
            var two = new ArbDecimal(2);
            ushort ii = 0;
            while (ii < max_iterations && LessThanFour(x * x + y * y))
            {
                var zed = x * x;
                var yarbo = y * y;
                var milo = zed - yarbo;
                var temp = milo + x0;

                var aaa = two * x;
                var bbb = aaa * y;
                bbb.CheckForZero();
                y = bbb + y0;
                x = temp;
                ii++;
            }
            return ii;
        }

        bool LessThanFour(ArbDecimal num)
        {
            // Figure out if this decimal is less than four, the quick way.

            // Numbers bigger than 1 slot are not less than four.
            if (num.decimal_point > 1)
                return false;

            // Numbers with no whole part are always less than four.
            else if (num.decimal_point == 0)
                return true;

            // The decimal point is one.  So is the number less than four?
            return num.digits[0] < 4;
        }
    }
}
