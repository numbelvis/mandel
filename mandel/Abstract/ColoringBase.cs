using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mandel
{
    public abstract class ColoringBase
    {
        #region Public

        public abstract Color Get(int iterations);

        #endregion
    }
}
