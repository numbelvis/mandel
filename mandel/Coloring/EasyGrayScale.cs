using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public class EasyGrayScale : ColoringBase
    {
        Color[] Map;

        public EasyGrayScale(int max_iterations)
            : base(max_iterations)
        {
            Map = new Color[max_iterations];

            var each = 255m / (decimal)max_iterations;
            for(var ii = 0; ii < max_iterations; ii++)
            {
                var num = (int)(each * ii);
                Map[ii] = Color.FromArgb(num, num, num);
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