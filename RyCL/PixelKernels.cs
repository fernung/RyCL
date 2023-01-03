using ILGPU;
using ILGPU.Runtime;

namespace RyCL
{
    public class PixelKernels
    {
        public Action<Index1D, int, DeviceBuffer> Fill;
        public Action<Index1D, DeviceBuffer, DeviceBuffer> CopyBuffer;
        public Action<Index1D, ArrayView<int>, DeviceBuffer> CopyRaw;
        public Action<Index1D, int, int, int, int, int, DeviceBuffer> Line;
        public Action<Index1D, int, int, int, int, int, int, int, DeviceBuffer> Triangle;
        public Action<Index1D, int, int, int, int, int, int, int, DeviceBuffer> TriangleFill;
        public Action<Index1D, int, int, int, int, int, DeviceBuffer> Rectangle;
        public Action<Index1D, int, int, int, int, int, DeviceBuffer> RectangleFill;
        public Action<Index1D, int, int, int, int, DeviceBuffer> Circle;
        public Action<Index1D, int, int, int, int, DeviceBuffer> CircleFill;

        public PixelKernels(Accelerator device)
        {
            Compile(device);
        }

        private void Compile(Accelerator device)
        {
            Fill = device.LoadAutoGroupedStreamKernel<Index1D, int, DeviceBuffer>(Kernel_Fill);
            CopyBuffer = device.LoadAutoGroupedStreamKernel<Index1D, DeviceBuffer, DeviceBuffer>(Kernel_Copy_Buffer);
            CopyRaw = device.LoadAutoGroupedStreamKernel<Index1D, ArrayView<int>, DeviceBuffer>(Kernel_Copy_Raw);
            Line = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Line);
            Triangle = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Triangle);
            TriangleFill = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Triangle_Fill);
            Rectangle = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Rectangle);
            RectangleFill = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Rectangle_Fill);
            Circle = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Circle);
            CircleFill = device.LoadAutoGroupedStreamKernel<Index1D, int, int, int, int, DeviceBuffer>(Kernel_Pixel_Circle_Fill);
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
        
        private static void Kernel_Pixel_Line(Index1D index, int x0, int y0, int x1, int y1, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(x0, x1);
            int maxX = IntrinsicMath.Max(x0, x1);
            int minY = IntrinsicMath.Min(y0, y1);
            int maxY = IntrinsicMath.Max(y0, y1);
            if (position.x < minX || position.x > maxX ||
                position.y < minY || position.y > maxY)
                return;

            double m = ((double)y0 - y1) / ((double)x0 - x1);
            double b = y0 - (m * x0);

            double distance = IntrinsicMath.Abs((m * position.x) - position.y + b) / Math.Sqrt((m * m) + 1);
            if (distance < 0.5)
                output.SetPixel(position.x, position.y, color);
        }
        
        private static void Kernel_Pixel_Triangle(Index1D index, int x0, int y0, int x1, int y1, int x2, int y2, int color, DeviceBuffer output)
        {
            Kernel_Pixel_Line(index, x0, y0, x1, y1, color, output);
            Kernel_Pixel_Line(index, x1, y1, x2, y2, color, output);
            Kernel_Pixel_Line(index, x2, y2, x0, y0, color, output);
        }
        private static void Kernel_Pixel_Triangle_Fill(Index1D index, int x0, int y0, int x1, int y1, int x2, int y2, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int w0 = (x1 - x2) * (position.y - y2) - (y1 - y2) * (position.x - x2);
            int w1 = (x2 - x0) * (position.y - y0) - (y2 - y0) * (position.x - x0);
            int w2 = (x0 - x1) * (position.y - y1) - (y0 - y1) * (position.x - x1);
            if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                output.SetPixel(position.x, position.y, color);
        }
        
        private static void Kernel_Pixel_Rectangle(Index1D index, int x0, int y0, int x1, int y1, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(x0, x1);
            int maxX = IntrinsicMath.Max(x0, x1);
            int minY = IntrinsicMath.Min(y0, y1);
            int maxY = IntrinsicMath.Max(y0, y1);
            if ((position.x == minX || position.x == maxX || position.y == minY || position.y == maxY) &&
                (minX <= position.x && position.x <= maxX && minY <= position.y && position.y <= maxY))
                output.SetPixel(position.x, position.y, color);
        }
        private static void Kernel_Pixel_Rectangle_Fill(Index1D index, int x0, int y0, int x1, int y1, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(x0, x1);
            int maxX = IntrinsicMath.Max(x0, x1);
            int minY = IntrinsicMath.Min(y0, y1);
            int maxY = IntrinsicMath.Max(y0, y1);
            if (minX <= position.x && position.x <= maxX && 
                minY <= position.y && position.y <= maxY)
                output.SetPixel(position.x, position.y, color);
        }

        private static void Kernel_Pixel_Circle(Index1D index, int x0, int y0, int radius, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            //int d = (position.x - x0) * (position.x - x0) + (position.y - y0) * (position.y - y0);
            //if (d < radius * radius && d > (radius - 1) * (radius - 1))
            double distance = Math.Sqrt((position.x - x0) * (position.x - x0) + (position.y - y0) * (position.y - y0));
            if (distance < radius || distance > radius + 0.98) return;
            output.SetPixel(position.x, position.y, color);
        }
        private static void Kernel_Pixel_Circle_Fill(Index1D index, int x0, int y0, int radius, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int xDelta = position.x - x0;
            int yDelta = position.y - y0;
            if (Math.Sqrt(xDelta * xDelta + yDelta * yDelta) <= radius)
                output.SetPixel(position.x, position.y, color);
        }
    }
}
