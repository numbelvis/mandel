using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public class WaveyColoring : ColoringBase
    {
        public override System.Drawing.Color Get(int iterations)
        {
            //return Color.Red;
            return Color.FromArgb(iterations, iterations, iterations);
        }
    }
}