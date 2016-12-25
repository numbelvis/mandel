using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public class WaveyColoring : ColoringBase
    {
        int red_factor;
        int green_factor;
        int blue_factor;

        Color[] Map;

        public WaveyColoring(int max_iterations)
            : base(max_iterations)
        {
            var randy = new Random(DateTime.Now.Millisecond * DateTime.Now.Second);
            red_factor = randy.Next(4, 50);
            green_factor = randy.Next(4, 50);
            blue_factor = randy.Next(4, 50);

            Map = new Color[max_iterations];

            for(var ii = 0; ii < max_iterations; ii++)
            {
                Map[ii] = Color.FromArgb((int)(128d * (1 + Math.Sin(ii * red_factor))),
                                        (int)(128d * (1 + Math.Sin(ii * green_factor))),
                                        (int)(128d * (1 + Math.Sin(ii * blue_factor))));
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