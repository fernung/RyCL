using ILGPU;
using ILGPU.Runtime;
using System.Drawing;

namespace RyCL.Graphics.Kernels
{
    public class RectangleKernel
    {
        public readonly Action<Index1D, RectangleDrawCall, DeviceBuffer> Rectangle;
        public readonly Action<Index2D, ArrayView<RectangleDrawCall>, DeviceBuffer> Rectangles;

        public readonly Action<Index1D, RectangleDrawCall, DeviceBuffer> FilledRectangle;
        public readonly Action<Index2D, ArrayView<RectangleDrawCall>, DeviceBuffer> FilledRectangles;

        public RectangleKernel(Accelerator device)
        {
            Rectangle = device.LoadAutoGroupedStreamKernel<Index1D, RectangleDrawCall, DeviceBuffer>(Kernel_Draw_Rectangle);
            Rectangles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<RectangleDrawCall>, DeviceBuffer>(Kernel_Draw_Rectangles);

            FilledRectangle = device.LoadAutoGroupedStreamKernel<Index1D, RectangleDrawCall, DeviceBuffer>(Kernel_Fill_Rectangle);
            FilledRectangles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<RectangleDrawCall>, DeviceBuffer>(Kernel_Fill_Rectangles);
        }

        private static void Kernel_Draw_Rectangle(Index1D index, RectangleDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(drawCall.X0, drawCall.X1);
            int maxX = IntrinsicMath.Max(drawCall.X0, drawCall.X1);
            int minY = IntrinsicMath.Min(drawCall.Y0, drawCall.Y1);
            int maxY = IntrinsicMath.Max(drawCall.Y0, drawCall.Y1);
            if ((position.x == minX || position.x == maxX || position.y == minY || position.y == maxY) &&
                (minX <= position.x && position.x <= maxX && minY <= position.y && position.y <= maxY))
                output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Draw_Rectangles(Index2D index, ArrayView<RectangleDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            int minX = IntrinsicMath.Min(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int maxX = IntrinsicMath.Max(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int minY = IntrinsicMath.Min(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            int maxY = IntrinsicMath.Max(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            if ((position.x == minX || position.x == maxX || position.y == minY || position.y == maxY) &&
                (minX <= position.x && position.x <= maxX && minY <= position.y && position.y <= maxY))
                output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }

        private static void Kernel_Fill_Rectangle(Index1D index, RectangleDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(drawCall.X0, drawCall.X1);
            int maxX = IntrinsicMath.Max(drawCall.X0, drawCall.X1);
            int minY = IntrinsicMath.Min(drawCall.Y0, drawCall.Y1);
            int maxY = IntrinsicMath.Max(drawCall.Y0, drawCall.Y1);
            if (minX <= position.x && position.x <= maxX &&
                minY <= position.y && position.y <= maxY)
                output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Fill_Rectangles(Index2D index, ArrayView<RectangleDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            int minX = IntrinsicMath.Min(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int maxX = IntrinsicMath.Max(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int minY = IntrinsicMath.Min(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            int maxY = IntrinsicMath.Max(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            if (minX <= position.x && position.x <= maxX &&
                minY <= position.y && position.y <= maxY)
                output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }
    }

    public struct RectangleDrawCall
    {
        public int X0;
        public int Y0;
        public int X1;
        public int Y1;
        public int Color;

        public RectangleDrawCall(int x0, int y0, int x1, int y1, int color)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            Color = color;
        }
    }
}
