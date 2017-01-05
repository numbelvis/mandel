using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public class SingleColor : ColoringBase
    {
        Color[] Map;

        public SingleColor(int max_iterations)
            : base(max_iterations)
        {
            Map = new Color[max_iterations];

            var bo_bandy = new Random(DateTime.Now.Millisecond);
            var red = bo_bandy.Next(5, 250);
            var blue = bo_bandy.Next(5, 250);
            var green = bo_bandy.Next(5, 250);

            var red_each = (decimal)red / (decimal)max_iterations;
            var blue_each = (decimal)blue / (decimal)max_iterations;
            var green_each = (decimal)green / (decimal)max_iterations;

            for (var ii = 0; ii < max_iterations; ii++)
            {
                Map[ii] = Color.FromArgb((int)(red_each * ii), (int)(green_each * ii), (int)(blue_each * ii));
            }
        }

        public override System.Drawing.Color Get(int iterations, int max_iterations)
        {
            var idx = iterations - 1;
            if (idx < 0)
                idx = 0;

            return Map[idx];
        }
    }
}