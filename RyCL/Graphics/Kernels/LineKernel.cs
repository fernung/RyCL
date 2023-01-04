using ILGPU;
using ILGPU.Runtime;

namespace RyCL.Graphics.Kernels
{
    public class LineKernel
    {
        public readonly Action<Index1D, LineDrawCall, DeviceBuffer> Line;
        public readonly Action<Index2D, ArrayView<LineDrawCall>, DeviceBuffer> Lines;

        public LineKernel(Accelerator device)
        {
            Line = device.LoadAutoGroupedStreamKernel<Index1D, LineDrawCall, DeviceBuffer>(Kernel_Draw_Line);
            Lines = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<LineDrawCall>, DeviceBuffer>(Kernel_Draw_Lines);
        }

        private static void Kernel_Draw_Line(Index1D index, LineDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(drawCall.X0, drawCall.X1);
            int maxX = IntrinsicMath.Max(drawCall.X0, drawCall.X1);
            int minY = IntrinsicMath.Min(drawCall.Y0, drawCall.Y1);
            int maxY = IntrinsicMath.Max(drawCall.Y0, drawCall.Y1);
            if (position.x < minX || position.x > maxX ||
                position.y < minY || position.y > maxY)
                return;

            double m = ((double)drawCall.Y0 - drawCall.Y1) / 
                       ((double)drawCall.X0 - drawCall.X1);
            double b = drawCall.Y0 - m * drawCall.X0;
            double d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
            if (d < 0.5)
                output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Draw_Lines(Index2D index, ArrayView<LineDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            int minX = IntrinsicMath.Min(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int minY = IntrinsicMath.Min(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            int maxX = IntrinsicMath.Max(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            int maxY = IntrinsicMath.Max(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            if (position.x < minX || position.x > maxX ||
                position.y < minY || position.y > maxY)
                return;

            double m = ((double)drawCalls[index.Y].Y0 - drawCalls[index.Y].Y1) /
                       ((double)drawCalls[index.Y].X0 - drawCalls[index.Y].X1);
            double b = drawCalls[index.Y].Y0 - m * drawCalls[index.Y].X0;
            double d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
            if (d < 0.5)
                output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }
    }

    public struct LineDrawCall
    {
        public int X0;
        public int Y0;
        public int X1;
        public int Y1;
        public int Color;

        public LineDrawCall(int x0, int y0, int x1, int y1, int color)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            Color = color;
        }
    }
}
