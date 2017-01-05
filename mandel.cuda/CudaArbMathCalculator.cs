using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes; 
using mandel.arb;

namespace mandel.cuda
{
    public class CudaArbMathCalculator : CalculatorBase<CudaArbMathCalculator, ArbDecimal>
    {
        #region Properties

        CudaContext Context;
        CudaKernel Kernel;

        #endregion


        #region Constructor

        public CudaArbMathCalculator(LocationBase<ArbDecimal> location, int output_width, int output_height)
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
                CUmodule module = this.Context.LoadModule("arbkernel.ptx");
                this.Kernel = new CudaKernel("_Z6kernelPtiiiS_iiS_iiS_iiS_i", module, Context);
            }
        }

        #endregion


        #region Abstracts

        public override int GetWidthDivisionCount()
        {
            EnsureSetup();

            if (this.OutputWidth < Kernel.MaxThreadsPerBlock)
                return 1;

            return (int)Math.Ceiling((decimal)this.OutputWidth / (decimal)Kernel.MaxThreadsPerBlock);
        }

        public override int GetWidthDivisionSize()
        {
            EnsureSetup();

            if (this.OutputWidth < Kernel.MaxThreadsPerBlock)
                return this.OutputWidth;

            return Kernel.MaxThreadsPerBlock;
        }

        public override ushort[] DoBlock(int x_start, int x_count, int y_start, int y_count, int max_iterations)
        {
            // The length of our results array.
            var result_length = x_count * y_count;

            // Get the points to start calculating at.
            ArbDecimal y0;
            ArbDecimal x0;
            this.Location.EmitPoints(out x0, out y0, x_start, y_start, this.ColumnWidth, this.LineHeight);

            // Set up the GPU's threads and blocks.  For threads, we use a one-dimensional array the length of x_count, which will never be greater than MaxThreadsPerBlock
            Kernel.BlockDimensions = new dim3(x_count);

            // Then the blocks are done 1 dimensionally as well with a width of the y_count.  
            // This y_count comes from the lines_per value.  To make the GPU work hard per cycle, increase the Lines Per value when calling the renderer.
            Kernel.GridDimensions = new dim3(1, y_count);


            // Tied this context to this thread.  Very important.
            this.Context.SetCurrent();

            // An array for us to copy the GPU's output into.
            var output = new ushort[result_length];

            // Create an array on the device based on the result array.
            var device_result = new CudaDeviceVariable<ushort>(result_length);

            // Copy digits to the device.
            var arb_x_offset_host_map_array = new CudaDeviceVariable<ushort>((int)ArbConstants.DigitArraySize);
            arb_x_offset_host_map_array.CopyToDevice(x0.digits);

            var arb_y_offset_host_map_array = new CudaDeviceVariable<ushort>((int)ArbConstants.DigitArraySize);
            arb_y_offset_host_map_array.CopyToDevice(y0.digits);

            var arb_x_point_scale_host_map_array = new CudaDeviceVariable<ushort>((int)ArbConstants.DigitArraySize);
            arb_x_point_scale_host_map_array.CopyToDevice(this.ColumnWidth.digits);

            var arb_y_point_scale_host_map_array = new CudaDeviceVariable<ushort>((int)ArbConstants.DigitArraySize);
            arb_y_point_scale_host_map_array.CopyToDevice(this.LineHeight.digits);
            


            // Run the kernel.  It will populate the device result array in device memory.
            this.Kernel.Run(

                device_result.DevicePointer,
                x_count,

                this.LineHeight.sign ? 1 : 0,
                this.LineHeight.decimal_point,
                arb_y_point_scale_host_map_array.DevicePointer,

                y0.sign ? 1 : 0,
                y0.decimal_point,
                arb_y_offset_host_map_array.DevicePointer,

                this.ColumnWidth.sign ? 1 : 0,
                this.ColumnWidth.decimal_point,
                arb_x_point_scale_host_map_array.DevicePointer,

                x0.sign ? 1 : 0,
                x0.decimal_point,
                arb_x_offset_host_map_array.DevicePointer,

                max_iterations);

            // Copy the device's result array back to the host (the computer) so we can use it.
            device_result.CopyToHost(output);

            return output;
        }

        #endregion
    }
}
