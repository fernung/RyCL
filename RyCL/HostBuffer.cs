using ILGPU.Runtime;
using ILGPU;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RyCL
{
    public class HostBuffer : IDisposable
    {
        private bool _disposed;
        private bool _syncCpu;
        private bool _syncGpu;

        private int _width;
        private int _height;

        private int[] _hostBuffer;
        private MemoryBuffer1D<int, Stride1D.Dense> _memoryBuffer;
        private DeviceBuffer _deviceBuffer;

        public int Width => _width;
        public int Height => _height;
        public int Count => _hostBuffer.Length;

        public int this[int i]
        {
            get
            {
                if (_syncCpu)
                {
                    _memoryBuffer.CopyToCPU(_hostBuffer);
                    _syncCpu = false;
                }
                return _hostBuffer[i];
            }
            set
            {
                _syncGpu = true;
                _hostBuffer[i] = value;
            }
        }
        public int this[int x, int y]
        {
            get => this[(y * _width) + x];
            set => this[(y * _width) + x] = value;
        }

        public HostBuffer(Accelerator device, int width, int height)
        {
            _disposed = false;
            _syncCpu = false;
            _syncGpu = false;

            _width = width;
            _height = height;

            _hostBuffer = new int[width * height];
            _memoryBuffer = device.Allocate1D<int>(_hostBuffer.Length);
            _deviceBuffer = new DeviceBuffer(width, height, _memoryBuffer);
        }

        public DeviceBuffer GetDeviceBuffer()
        {
            if (_syncGpu)
            {
                _memoryBuffer.CopyFromCPU(_hostBuffer);
                _syncGpu = false;
            }
            _syncCpu = true;
            return _deviceBuffer;
        }
        public ref int[] GetHostBufferRaw()
        {
            if (_syncCpu)
            {
                _memoryBuffer.CopyToCPU(_hostBuffer);
                _syncCpu = false;
            }
            return ref _hostBuffer;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _memoryBuffer?.Dispose();
        }
    }
    public struct DeviceBuffer
    {
        public int Width;
        public int Height;
        public ArrayView1D<int, Stride1D.Dense> Buffer;

        public DeviceBuffer(int width, int height, MemoryBuffer1D<int, Stride1D.Dense> buffer)
        {
            Width = width;
            Height = height;
            Buffer = buffer.View;
        }

        public int Index(int x, int y) =>
            (y * Width) + x;
        public (int x, int y) Index(int i) =>
            (i % Width, i / Width);

        public int GetPixel(int i)
        {
            return Buffer[i];
        }
        public void SetPixel(int i, int value)
        {
            Buffer[i] = value;
        }

        public int GetPixel(int x, int y)
        {
            return Buffer[Index(x, y)];
        }
        public void SetPixel(int x, int y, int value)
        {
            Buffer[Index(x, y)] = value;
        }
    }
}