using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel
{
    public class Location : LocationBase<MDecimal>
    {
        public Location(MDecimal x0, MDecimal xMax, MDecimal y0, MDecimal yMax)
            : base(x0, xMax, y0, yMax)
        { }

        public override void EmitPoints(out MDecimal x0, out MDecimal y0, int x, int y, MDecimal col_width, MDecimal line_height)
        {
            x0 = new MDecimal(0);
            y0 = new MDecimal(0);

            x0.value = this.x0.value + x * col_width.value;
            y0.value = this.y0.value + y * line_height.value;
        }

        public override MDecimal CalculateLineHeight(int output_height)
        {
            var diff = this.yMax.value - this.y0.value;
            var each_line = diff / Convert.ToDecimal(output_height);

            return new MDecimal(each_line);
        }

        public override MDecimal CalculateColumnWidth(int output_width)
        {
            var diff = this.xMax.value - this.x0.value;
            var each_line = diff / Convert.ToDecimal(output_width);

            return new MDecimal(each_line);
        }
    }
}
