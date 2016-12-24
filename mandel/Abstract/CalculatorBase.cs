using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace mandel
{
    public abstract class CalculatorBase<T>
        where T : class, new()
    {
        #region Abstracts

        #endregion


        #region Public

        public ushort[] CalculateBlock(ILocation location, int image_y0, int image_y, int max_iterations)
        {
            var result = new ushort[MandelConstants.DigitArraySize];

            return result;
        }
        
        #endregion


        #region Private



        #endregion
    }
}
