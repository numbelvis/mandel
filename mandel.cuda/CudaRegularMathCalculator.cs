using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandel.cuda
{
    public class CudaRegularMathCalculator : CalculatorBase<CudaRegularMathCalculator, MDecimal>
    {
        public CudaRegularMathCalculator(LocationBase<MDecimal> location, int output_width, int output_height)
            : base(location, output_width, output_height)
        { }

        public override int GetWidthDivisionCount()
        {
            return (int)Math.Floor(Convert.ToDecimal(this.OutputWidth) / Convert.ToDecimal(Constants.GPU_Blocks_x * Constants.GPU_Threads_x));
        }

        public override int GetWidthDivisionSize()
        {
            return Constants.GPU_Blocks_x * Constants.GPU_Threads_x;
        }

        public override ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations)
        {
            throw new NotImplementedException();
        }
    }
}
