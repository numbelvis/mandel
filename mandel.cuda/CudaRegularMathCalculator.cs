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
            EnsureSetup();
        }

        void EnsureSetup()
        {
            if (this.Context == null)
            {
                this.Context = new CudaContext(Constants.GPU_DeviceId, true);

                // The ptx file must be in the same folder as the executing file.
                CUmodule module = this.Context.LoadModule("kernel.ptx");
                this.Kernel = new CudaKernel("_Z6kernelPiiddddi", module, Context);
            }
        }

        #endregion


        #region Abstracts

        public override int GetWidthDivisionCount()
        {
            EnsureSetup();
            return (int)Math.Ceiling((decimal)this.OutputWidth / (decimal)Kernel.MaxThreadsPerBlock);
        }

        public override int GetWidthDivisionSize()
        {
            EnsureSetup();
            return Kernel.MaxThreadsPerBlock;
        }

        public override ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations)
        {
            // The length of our results array.
            var result_length = x_count * y_count;

            // Get the points to start calculating at.
            MDecimal y0;
            MDecimal x0;
            this.Location.EmitPoints(out x0, out y0, x_start, y_start, this.ColumnWidth, this.LineHeight);

            // Set up the GPU's threads and blocks.  For threads, we use a one-dimensional array with a length of the max threads the GPU allows per block.
            Kernel.BlockDimensions = new dim3(Kernel.MaxThreadsPerBlock);

            // Then the blocks are done 1 dimensionally as well with a width of the y_count.  
            // This y_count comes from the lines_per value.  To make the GPU worked hard per cycle, increase the Lines Per value when calling the renderer.
            Kernel.GridDimensions = new dim3(1, y_count);


            // Tied this context to this thread.  Very important.
            this.Context.SetCurrent();

            // An array for us to copy the GPU's output into.
            var output = new int[result_length];

            // Create an array on the device based on the result array.
            var device_result = new CudaDeviceVariable<int>(result_length);

            // Run the kernel.  It will populate the device result array in device memory.
            this.Kernel.Run(device_result.DevicePointer, x_count, Convert.ToDouble(this.LineHeight.value), Convert.ToDouble(y0.value), Convert.ToDouble(this.ColumnWidth.value), Convert.ToDouble(x0.value), max_iterations);

            // Copy the device's result array back to the host (the computer) so we can use it.
            device_result.CopyToHost(output);

            // Convert to ushorts.  The result should all be converted to use ushort to eliminate this step.
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
