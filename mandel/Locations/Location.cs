using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel
{
    public class Location : ILocation
    {
        #region Properties

        public double x0
        { get; set; }

        public double y0
        { get; set; }

        public double xMax
        { get; set; }

        public double yMax
        { get; set; }

        #endregion
    }
}
