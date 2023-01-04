using ILGPU;
using ILGPU.Runtime;

namespace RyCL.Graphics.Kernels
{
    public class TriangleKernel
    {
        public readonly Action<Index1D, TriangleDrawCall, DeviceBuffer> Triangle;
        public readonly Action<Index2D, ArrayView<TriangleDrawCall>, DeviceBuffer> Triangles;

        public readonly Action<Index1D, TriangleDrawCall, DeviceBuffer> FilledTriangle;
        public readonly Action<Index2D, ArrayView<TriangleDrawCall>, DeviceBuffer> FilledTriangles;

        public TriangleKernel(Accelerator device)
        {
            Triangle = device.LoadAutoGroupedStreamKernel<Index1D, TriangleDrawCall, DeviceBuffer>(Kernel_Draw_Triangle);
            Triangles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<TriangleDrawCall>, DeviceBuffer>(Kernel_Draw_Triangles);

            FilledTriangle = device.LoadAutoGroupedStreamKernel<Index1D, TriangleDrawCall, DeviceBuffer>(Kernel_Fill_Triangle);
            FilledTriangles = device.LoadAutoGroupedStreamKernel<Index2D, ArrayView<TriangleDrawCall>, DeviceBuffer>(Kernel_Fill_Triangles);
        }

        private static void Kernel_Draw_Triangle(Index1D index, TriangleDrawCall drawCall, DeviceBuffer output)
        {
            Kernel_Draw_Line(index, drawCall.X0, drawCall.Y0, drawCall.X1, drawCall.Y1, drawCall.Color, output);
            Kernel_Draw_Line(index, drawCall.X1, drawCall.Y1, drawCall.X2, drawCall.Y2, drawCall.Color, output);
            Kernel_Draw_Line(index, drawCall.X2, drawCall.Y2, drawCall.X0, drawCall.Y0, drawCall.Color, output);
        }
        private static void Kernel_Draw_Triangles(Index2D index, ArrayView<TriangleDrawCall> drawCalls, DeviceBuffer output)
        {
            Kernel_Draw_Line(index.X, drawCalls[index.Y].X0, drawCalls[index.Y].Y0, drawCalls[index.Y].X1, drawCalls[index.Y].Y1, drawCalls[index.Y].Color, output);
            Kernel_Draw_Line(index.X, drawCalls[index.Y].X1, drawCalls[index.Y].Y1, drawCalls[index.Y].X2, drawCalls[index.Y].Y2, drawCalls[index.Y].Color, output);
            Kernel_Draw_Line(index.X, drawCalls[index.Y].X2, drawCalls[index.Y].Y2, drawCalls[index.Y].X0, drawCalls[index.Y].Y0, drawCalls[index.Y].Color, output);
        }

        private static void Kernel_Fill_Triangle(Index1D index, TriangleDrawCall drawCall, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int w0 = (drawCall.X1 - drawCall.X2) * (position.y - drawCall.Y2) - (drawCall.Y1 - drawCall.Y2) * (position.x - drawCall.X2);
            int w1 = (drawCall.X2 - drawCall.X0) * (position.y - drawCall.Y0) - (drawCall.Y2 - drawCall.Y0) * (position.x - drawCall.X0);
            int w2 = (drawCall.X0 - drawCall.X1) * (position.y - drawCall.Y1) - (drawCall.Y0 - drawCall.Y1) * (position.x - drawCall.X1);
            if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                output.SetPixel(position.x, position.y, drawCall.Color);
        }
        private static void Kernel_Fill_Triangles(Index2D index, ArrayView<TriangleDrawCall> drawCalls, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index.X);
            int w0 = (drawCalls[index.Y].X1 - drawCalls[index.Y].X2) * (position.y - drawCalls[index.Y].Y2) - (drawCalls[index.Y].Y1 - drawCalls[index.Y].Y2) * (position.x - drawCalls[index.Y].X2);
            int w1 = (drawCalls[index.Y].X2 - drawCalls[index.Y].X0) * (position.y - drawCalls[index.Y].Y0) - (drawCalls[index.Y].Y2 - drawCalls[index.Y].Y0) * (position.x - drawCalls[index.Y].X0);
            int w2 = (drawCalls[index.Y].X0 - drawCalls[index.Y].X1) * (position.y - drawCalls[index.Y].Y1) - (drawCalls[index.Y].Y0 - drawCalls[index.Y].Y1) * (position.x - drawCalls[index.Y].X1);
            if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
        }

        private static void Kernel_Draw_Line(Index1D index, int x0, int y0, int x1, int y1, int color, DeviceBuffer output)
        {
            (int x, int y) position = output.Index(index);
            int minX = IntrinsicMath.Min(x0, x1);
            int maxX = IntrinsicMath.Max(x0, x1);
            int minY = IntrinsicMath.Min(y0, y1);
            int maxY = IntrinsicMath.Max(y0, y1);
            if (position.x < minX || position.x > maxX ||
                position.y < minY || position.y > maxY)
                return;

            double m = ((double)y0 - y1) /
                       ((double)x0 - x1);
            double b = y0 - m * x0;
            double d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
            if (d < 0.5)
                output.SetPixel(position.x, position.y, color);
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

    public struct TriangleDrawCall
    {
        public int X0;
        public int Y0;
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        public int Color;

        public TriangleDrawCall(int x0, int y0, int x1, int y1, int x2, int y2, int color)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Color = color;
        }
    }
}
