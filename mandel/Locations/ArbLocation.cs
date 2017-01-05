using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mandel.arb;

namespace mandel
{
    public class ArbLocation : LocationBase<ArbDecimal>
    {
        public ArbLocation(ArbDecimal x0, ArbDecimal xMax, ArbDecimal y0, ArbDecimal yMax)
            : base(x0, xMax, y0, yMax)
        { }

        public override void EmitPoints(out ArbDecimal x0, out ArbDecimal y0, int x, int y, ArbDecimal col_width, ArbDecimal line_height)
        {
            x0 = this.x0 + new ArbDecimal(x) * col_width;
            y0 = this.y0 + new ArbDecimal(y) * line_height;
        }

        public override ArbDecimal CalculateLineHeight(int output_height)
        {
            var diff = this.yMax - this.y0;

            // This is the one place we need division, but it is against an integer so we can do the division in regular math and avoid an arbitrary division routine.
            var each_line = diff * new ArbDecimal(1m / Convert.ToDecimal(output_height));
            return each_line;
        }

        public override ArbDecimal CalculateColumnWidth(int output_width)
        {
            var diff = xMax - x0;
            var each_line = diff * new ArbDecimal(1m / Convert.ToDecimal(output_width));
            return each_line;
        }
    }
}
