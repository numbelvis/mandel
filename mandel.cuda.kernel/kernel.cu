#include "cuda_runtime.h"
#include "device_launch_parameters.h"

/*

	Kernel for use with ManagedCuda

*/
__global__ void kernel(int *c, int xsize, double y_scale, double y_base, double x_scale, double x_base, int max)
{
	int idx_x = blockDim.x * blockIdx.x + threadIdx.x;
	int idx_y = blockDim.y * blockIdx.y + threadIdx.y;
	int idx = xsize * idx_y + idx_x;

	double x0 = (x_base + ((double)idx_x * x_scale));
	double y0 = (y_base + ((double)idx_y * y_scale));
	double x = 0;
	double y = 0;
	int ii = 0;
	double temp = 0;
	while (ii < max && x * x + y * y < 4)
	{
		temp = x * x - y * y + x0;
		y = x * 2 * y + y0;
		x = temp;
		ii++;
	}


	c[idx] = ii;
}

int main()
{
	return 0;
}