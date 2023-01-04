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
            int minX, minY, maxX, maxY;
            double m, b, d;
            (int x, int y) position = output.Index(index);

            // Vertex 0: (x0, y0)->(x1, y1)
            minX = IntrinsicMath.Min(drawCall.X0, drawCall.X1);
            maxX = IntrinsicMath.Max(drawCall.X0, drawCall.X1);
            minY = IntrinsicMath.Min(drawCall.Y0, drawCall.Y1);
            maxY = IntrinsicMath.Max(drawCall.Y0, drawCall.Y1);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCall.Y0 - drawCall.Y1) /
                ((double)drawCall.X0 - drawCall.X1);
                b = drawCall.Y0 - m * drawCall.X0;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCall.Color);
            }

            // Vertex 1: (x1, y1)->(x2, y2)
            minX = IntrinsicMath.Min(drawCall.X1, drawCall.X2);
            maxX = IntrinsicMath.Max(drawCall.X1, drawCall.X2);
            minY = IntrinsicMath.Min(drawCall.Y1, drawCall.Y2);
            maxY = IntrinsicMath.Max(drawCall.Y1, drawCall.Y2);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCall.Y1 - drawCall.Y2) /
                ((double)drawCall.X1 - drawCall.X2);
                b = drawCall.Y1 - m * drawCall.X1;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCall.Color);
            }

            // Vertex 2: (x2, y2)->(x0, y0)
            minX = IntrinsicMath.Min(drawCall.X2, drawCall.X0);
            maxX = IntrinsicMath.Max(drawCall.X2, drawCall.X0);
            minY = IntrinsicMath.Min(drawCall.Y2, drawCall.Y0);
            maxY = IntrinsicMath.Max(drawCall.Y2, drawCall.Y0);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCall.Y2 - drawCall.Y0) /
                ((double)drawCall.X2 - drawCall.X0);
                b = drawCall.Y2 - m * drawCall.X2;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCall.Color);
            }
        }
        private static void Kernel_Draw_Triangles(Index2D index, ArrayView<TriangleDrawCall> drawCalls, DeviceBuffer output)
        {
            int minX, minY, maxX, maxY;
            double m, b, d;
            (int x, int y) position = output.Index(index.X);

            // Vertex 0: (x0, y0)->(x1, y1)
            minX = IntrinsicMath.Min(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            maxX = IntrinsicMath.Max(drawCalls[index.Y].X0, drawCalls[index.Y].X1);
            minY = IntrinsicMath.Min(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            maxY = IntrinsicMath.Max(drawCalls[index.Y].Y0, drawCalls[index.Y].Y1);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCalls[index.Y].Y0 - drawCalls[index.Y].Y1) /
                ((double)drawCalls[index.Y].X0 - drawCalls[index.Y].X1);
                b = drawCalls[index.Y].Y0 - m * drawCalls[index.Y].X0;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
            }

            // Vertex 1: (x1, y1)->(x2, y2)
            minX = IntrinsicMath.Min(drawCalls[index.Y].X1, drawCalls[index.Y].X2);
            maxX = IntrinsicMath.Max(drawCalls[index.Y].X1, drawCalls[index.Y].X2);
            minY = IntrinsicMath.Min(drawCalls[index.Y].Y1, drawCalls[index.Y].Y2);
            maxY = IntrinsicMath.Max(drawCalls[index.Y].Y1, drawCalls[index.Y].Y2);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCalls[index.Y].Y1 - drawCalls[index.Y].Y2) /
                ((double)drawCalls[index.Y].X1 - drawCalls[index.Y].X2);
                b = drawCalls[index.Y].Y1 - m * drawCalls[index.Y].X1;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
            }

            // Vertex 2: (x2, y2)->(x0, y0)
            minX = IntrinsicMath.Min(drawCalls[index.Y].X2, drawCalls[index.Y].X0);
            maxX = IntrinsicMath.Max(drawCalls[index.Y].X2, drawCalls[index.Y].X0);
            minY = IntrinsicMath.Min(drawCalls[index.Y].Y2, drawCalls[index.Y].Y0);
            maxY = IntrinsicMath.Max(drawCalls[index.Y].Y2, drawCalls[index.Y].Y0);
            if (position.x >= minX && position.x <= maxX &&
                position.y >= minY && position.y <= maxY)
            {
                m = ((double)drawCalls[index.Y].Y2 - drawCalls[index.Y].Y0) /
                ((double)drawCalls[index.Y].X2 - drawCalls[index.Y].X0);
                b = drawCalls[index.Y].Y2 - m * drawCalls[index.Y].X2;
                d = IntrinsicMath.Abs(m * position.x - position.y + b) / Math.Sqrt(m * m + 1);
                if (d < 0.5)
                    output.SetPixel(position.x, position.y, drawCalls[index.Y].Color);
            }
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
