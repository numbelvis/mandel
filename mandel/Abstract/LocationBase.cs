using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mandel
{
    public abstract class LocationBase<Tnumber>
        where Tnumber : class
    {
        public abstract void EmitPoints(out Tnumber x0, out Tnumber y0, int x, int y, Tnumber col_width, Tnumber line_height);

        public abstract Tnumber CalculateLineHeight(int output_height);

        public abstract Tnumber CalculateColumnWidth(int output_width);

        public Tnumber x0
        { get; set; }

        public Tnumber y0
        { get; set; }

        public Tnumber xMax
        { get; set; }

        public Tnumber yMax
        { get; set; }

        public decimal RateOfDescent
        { get; set; }

        public LocationBase(Tnumber x0, Tnumber xMax, Tnumber y0, Tnumber yMax)
        {
            this.x0 = x0;
            this.xMax = xMax;
            this.y0 = y0;
            this.yMax = yMax;
        }
    }
}
