using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public abstract class ColoringBase
    {
        public abstract Color Get(int iterations, int max_iterations);

        public int MaxIterations;

        public ColoringBase(int max_iterations)
        {
            this.MaxIterations = max_iterations;
        }
    }
}
