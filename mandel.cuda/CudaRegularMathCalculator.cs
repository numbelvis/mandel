using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes; 

namespace mandel.cuda
{
    public class CudaRegularMathCalculator : CalculatorBase<CudaRegularMathCalculator, MDecimal>
    {
        #region Properties

        CudaContext Context;
        CudaKernel Kernel;

        #endregion


        #region Constructor

        public CudaRegularMathCalculator(LocationBase<MDecimal> location, int output_width, int output_height)
            : base(location, output_width, output_height)
        {
            this.Context = new CudaContext(Constants.GPU_DeviceId, true);

            // The ptx file must be in the same folder as the executing file.
            CUmodule module = this.Context.LoadModule("kernel.ptx");
            Kernel = new CudaKernel("_Z6kernelPiiddddi", module, Context);
        }

        #endregion


        #region Abstracts

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
            var result_length = x_count * y_count;

            MDecimal y0;
            MDecimal x0;
            this.Location.EmitPoints(out x0, out y0, x_start, y_start, this.ColumnWidth, this.LineHeight);

            Kernel.BlockDimensions = new dim3(x_count, y_count);
            Kernel.GridDimensions = new dim3(1, 1);

            this.Context.SetCurrent();

            var output = new int[result_length];
            var device_result = new CudaDeviceVariable<int>(result_length);

            this.Kernel.Run(device_result.DevicePointer, x_count, Convert.ToDouble(this.LineHeight.value), Convert.ToDouble(y0.value), Convert.ToDouble(this.ColumnWidth.value), Convert.ToDouble(x0.value), max_iterations);

            device_result.CopyToHost(output);

            var result = new ushort[result_length];
            for (var ii = 0; ii < result_length; ii++)
            {
                result[ii] = (ushort)output[ii];
            }
            return result;
        }

        #endregion
    }
}
