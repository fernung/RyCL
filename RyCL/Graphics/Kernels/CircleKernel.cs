using ILGPU;
using ILGPU.Runtime;
using System.Drawing;

namespace RyCL.Graphics.Kernels
{
    public class CircleKernel
    {
        public readonly Action<Index1D, CircleDrawCall, DeviceBuffer> Circle;
        public readonly Action<Index2D, ArrayView<CircleDrawCall>, DeviceBuffer> Circles;

        public readonly Action<Index1D, CircleDrawCall, DeviceBuffer> FilledCircle;
        public readonly Action<Index2D, ArrayView<CircleDrawCall>, DeviceBuffer> FilledCircles;

        public CircleKernel(Accelerator device)
        {
            Circle = device.LoadAutoGroupedStreamKernel<Index1D, CircleDrawCall, DeviceBuffer>(Kernel_Draw_Circle);
            Circles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<CircleDrawCall>, DeviceBuffer>(Kernel_Draw_Circles);

            FilledCircle = device.LoadAutoGroupedStreamKernel<Index1D, CircleDrawCall, DeviceBuffer>(Kernel_Fill_Circle);
            FilledCircles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<CircleDrawCall>, DeviceBuffer>(Kernel_Fill_Circles);
        }

        private static void Kernel_Draw_Circle(Index1D index, CircleDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            double distance = Math.Sqrt((position.x - drawCall.X) * (position.x - drawCall.X) + (position.y - drawCall.Y) * (position.y - drawCall.Y));
            if (distance < drawCall.Radius || distance > drawCall.Radius + 0.98) return;
            output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Draw_Circles(Index2D index, ArrayView<CircleDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            double distance = Math.Sqrt((position.x - drawCalls[index.Y].X) * (position.x - drawCalls[index.Y].X) + (position.y - drawCalls[index.Y].Y) * (position.y - drawCalls[index.Y].Y));
            if (distance < drawCalls[index.Y].Radius || distance > drawCalls[index.Y].Radius + 0.98) return;
            output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }

        private static void Kernel_Fill_Circle(Index1D index, CircleDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int xDelta = position.x - drawCall.X;
            int yDelta = position.y - drawCall.Y;
            if (Math.Sqrt(xDelta * xDelta + yDelta * yDelta) <= drawCall.Radius)
                output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Fill_Circles(Index2D index, ArrayView<CircleDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            int xDelta = position.x - drawCalls[index.Y].X;
            int yDelta = position.y - drawCalls[index.Y].Y;
            if (Math.Sqrt(xDelta * xDelta + yDelta * yDelta) <= drawCalls[index.Y].Radius)
                output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }
    }

    public struct CircleDrawCall
    {
        public int X;
        public int Y;
        public int Radius;
        public int Color;

        public CircleDrawCall(int x, int y, int radius, int color)
        {
            X = x;
            Y = y;
            Radius = radius;
            Color = color;
        }
    }
}
