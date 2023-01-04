using ILGPU;
using ILGPU.Algorithms.Random;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.OpenCL;
using RyCL.Graphics.Kernels;

namespace RyCL.Graphics
{
    public class RasterGL : IDisposable
    {
        private bool _disposed;

        private readonly Context _context;
        private readonly Accelerator _device;
        private readonly HostBuffer _buffer;

        private readonly PixelKernel _pixelKernel;
        private readonly LineKernel _lineKernel;
        private readonly TriangleKernel _triangleKernel;
        private readonly RectangleKernel _rectangleKernel;
        private readonly CircleKernel _circleKernel;

        private readonly List<LineDrawCall> _lineDrawCalls;
        private readonly List<TriangleDrawCall> _triangleDrawCalls;
        private readonly List<TriangleDrawCall> _triangleFillCalls;
        private readonly List<RectangleDrawCall> _rectangleDrawCalls;
        private readonly List<RectangleDrawCall> _rectangleFillCalls;
        private readonly List<CircleDrawCall> _circleDrawCalls;
        private readonly List<CircleDrawCall> _circleFillCalls;

        public Accelerator Device =>
            _device;
        public HostBuffer Buffer =>
            _buffer;

        public RasterGL(int width, int height, bool useCpu = false)
        {
            _context = Context.Create(builder => builder.OpenCL().CPU().EnableAlgorithms());
            _device = _context.GetPreferredDevice(useCpu).CreateAccelerator(_context);
            _buffer = new HostBuffer(_device, width, height);

            _pixelKernel = new PixelKernel(_device);
            _lineKernel = new LineKernel(_device);
            _triangleKernel = new TriangleKernel(_device);
            _rectangleKernel = new RectangleKernel(_device);
            _circleKernel = new CircleKernel(_device);

            _lineDrawCalls = new List<LineDrawCall>();
            _triangleDrawCalls = new List<TriangleDrawCall>();
            _triangleFillCalls = new List<TriangleDrawCall>();
            _rectangleDrawCalls = new List<RectangleDrawCall>();
            _rectangleFillCalls = new List<RectangleDrawCall>();
            _circleDrawCalls = new List<CircleDrawCall>();
            _circleFillCalls = new List<CircleDrawCall>();
        }

        public void Fill(int color)
        {
            _pixelKernel.Fill(_buffer.Count, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void FillRandom()
        {
            var rng = RNG.Create<XorShift128Plus>(_device, new Random());
            var randomBuffer = _device.Allocate1D<int>(_buffer.Count);
            rng.FillUniform(_device.DefaultStream, randomBuffer.View);

            _pixelKernel.CopyRaw(_buffer.Count, randomBuffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();

            randomBuffer?.Dispose();
            rng?.Dispose();
        }
        public void Copy(HostBuffer buffer)
        {
            _pixelKernel.CopyBuffer(buffer.Count, buffer.GetDeviceBuffer(), _buffer.GetDeviceBuffer());
        }

        public void DrawLine(int x0, int y0, int x1, int y1, int color)
        {
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            var drawCall = new LineDrawCall(x0, y0, x1, y1, color);
            _lineDrawCalls.Add(drawCall);
        }
        public void DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2, int color)
        {
            if (y0 == y1 && y0 == y2)
                return;
            if (y0 > y1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            if (y0 > y2)
            {
                (x0, x2) = (x2, x0);
                (y0, y2) = (y2, y0);
            }
            if (y1 > y2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }
            var drawCall = new TriangleDrawCall(x0, y0, x1, y1, x2, y2, color);
            _triangleDrawCalls.Add(drawCall);
        }
        public void FillTriangle(int x0, int y0, int x1, int y1, int x2, int y2, int color)
        {
            if (y0 == y1 && y0 == y2)
                return;
            if (y0 > y1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            if (y0 > y2)
            {
                (x0, x2) = (x2, x0);
                (y0, y2) = (y2, y0);
            }
            if (y1 > y2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }
            var drawCall = new TriangleDrawCall(x0, y0, x1, y1, x2, y2, color);
            _triangleFillCalls.Add(drawCall);
        }
        public void DrawRectangle(int x0, int y0, int x1, int y1, int color)
        {
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            var drawCall = new RectangleDrawCall(x0, y0, x1, y1, color);
            _rectangleDrawCalls.Add(drawCall);
        }
        public void FillRectangle(int x0, int y0, int x1, int y1, int color)
        {
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            var drawCall = new RectangleDrawCall(x0, y0, x1, y1, color);
            _rectangleFillCalls.Add(drawCall);
        }
        public void DrawCircle(int x, int y, int radius, int color)
        {
            var drawCall = new CircleDrawCall(x, y, radius, color);
            _circleDrawCalls.Add(drawCall);
        }
        public void FillCircle(int x, int y, int radius, int color)
        {
            var drawCall = new CircleDrawCall(x, y, radius, color);
            _circleFillCalls.Add(drawCall);
        }

        public void Draw()
        {
            DrawLines();
            DrawTriangles();
            FillTriangles();
            DrawRectangles();
            FillRectangles();
            DrawCircles();
            FillCircles();
        }
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _buffer?.Dispose();
            _device?.Dispose();
            _context?.Dispose();
        }

        private void DrawLines()
        {
            if (_lineDrawCalls.Count == 0)
                return;
            using MemoryBuffer1D<LineDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_lineDrawCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, _lineDrawCalls.Count);
            _lineKernel.Lines(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _lineDrawCalls.Clear();
        }
        private void DrawTriangles()
        {
            var count = _triangleDrawCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<TriangleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_triangleDrawCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _triangleKernel.Triangles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _triangleDrawCalls.Clear();
        }
        private void FillTriangles()
        {
            var count = _triangleFillCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<TriangleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_triangleFillCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _triangleKernel.FilledTriangles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _triangleFillCalls.Clear();
        }
        private void DrawRectangles()
        {
            var count = _rectangleDrawCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<RectangleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_rectangleDrawCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _rectangleKernel.Rectangles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _rectangleDrawCalls.Clear();
        }
        private void FillRectangles()
        {
            var count = _rectangleFillCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<RectangleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_rectangleFillCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _rectangleKernel.FilledRectangles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _rectangleFillCalls.Clear();
        }
        private void DrawCircles()
        {
            var count = _circleDrawCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<CircleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_circleDrawCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _circleKernel.Circles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _circleDrawCalls.Clear();
        }
        private void FillCircles()
        {
            var count = _circleFillCalls.Count;
            if (count == 0)
                return;
            using MemoryBuffer1D<CircleDrawCall, Stride1D.Dense> buffer = _device.Allocate1D(_circleFillCalls.ToArray());
            Index2D index = new Index2D(_buffer.Count, count);
            _circleKernel.FilledCircles(index, buffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();
            _circleFillCalls.Clear();
        }
    }
}
