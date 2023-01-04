using ILGPU;
using ILGPU.Runtime;

namespace RyCL.Graphics.Kernels
{
    public class PixelKernel
    {
        public readonly Action<Index1D, int, DeviceBuffer> Fill;
        public readonly Action<Index1D, DeviceBuffer, DeviceBuffer> CopyBuffer;
        public readonly Action<Index1D, ArrayView<int>, DeviceBuffer> CopyRaw;

        public PixelKernel(Accelerator device)
        {
            Fill = device.LoadAutoGroupedStreamKernel<Index1D, int, DeviceBuffer>(Kernel_Fill);
            CopyBuffer = device.LoadAutoGroupedStreamKernel<Index1D, DeviceBuffer, DeviceBuffer>(Kernel_Copy_Buffer);
            CopyRaw = device.LoadAutoGroupedStreamKernel<Index1D, ArrayView<int>, DeviceBuffer>(Kernel_Copy_Raw);
        }

        private static void Kernel_Fill(Index1D index, int value, DeviceBuffer output)
        {
            output.SetPixel(index, value);
        }
        private static void Kernel_Copy_Buffer(Index1D index, DeviceBuffer input, DeviceBuffer output)
        {
            output.SetPixel(index, input.GetPixel(index));
        }
        private static void Kernel_Copy_Raw(Index1D index, ArrayView<int> input, DeviceBuffer output)
        {
            output.SetPixel(index, input[index] | (0xFF << 24));
        }
    }
}
