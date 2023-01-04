using ILGPU;
using ILGPU.Algorithms.Random;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.OpenCL;

namespace RyCL
{
    public class PixelGL : IDisposable
    {
        private bool _disposed;

        private Context _context;
        private Accelerator _device;
        private HostBuffer _buffer;
        private PixelKernels _kernel;

        public Accelerator Device => 
            _device;
        public HostBuffer Buffer => 
            _buffer;
        public PixelKernels Kernel => 
            _kernel;

        public PixelGL(int width, int height, bool useCpu = false)
        {
            _context = Context.Create(builder => builder.OpenCL().CPU().EnableAlgorithms());
            _device = _context.GetPreferredDevice(useCpu).CreateAccelerator(_context);
            _buffer = new HostBuffer(_device, width, height);
            _kernel = new PixelKernels(_device);
        }

        public void Fill(int color)
        {
            _kernel.Fill(_buffer.Count, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void FillRandom()
        {
            var rng = RNG.Create<XorShift128Plus>(_device, new Random());
            var randomBuffer = _device.Allocate1D<int>(_buffer.Count);
            rng.FillUniform(_device.DefaultStream, randomBuffer.View);

            _kernel.CopyRaw(_buffer.Count, randomBuffer.View, _buffer.GetDeviceBuffer());
            _device.Synchronize();

            randomBuffer?.Dispose();
            rng?.Dispose();
        }

        public void DrawLine(int x0, int y0, int x1, int y1, int color)
        {
            if(x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            _kernel.Line(_buffer.Count, x0, y0, x1, y1, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
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
            _kernel.Triangle(_buffer.Count, x0, y0, x1, y1, x2, y2, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void DrawRectangle(int x0, int y0, int x1, int y1, int color)
        {
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            _kernel.Rectangle(_buffer.Count, x0, y0, x1, y1, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void DrawCircle(int x, int y, int radius, int color)
        {
            _kernel.Circle(_buffer.Count, x, y, radius, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
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
            _kernel.TriangleFill(_buffer.Count, x0, y0, x1, y1, x2, y2, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void FillRectangle(int x0, int y0, int x1, int y1, int color)
        {
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            _kernel.RectangleFill(_buffer.Count, x0, y0, x1, y1, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
        }
        public void FillCircle(int x, int y, int radius, int color)
        {
            _kernel.CircleFill(_buffer.Count, x, y, radius, color, _buffer.GetDeviceBuffer());
            _device.Synchronize();
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
    }
}
