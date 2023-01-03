using ILGPU;
using ILGPU.Algorithms.Random;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.OpenCL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace RyCL.Runner.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var pixelGL = new PixelGL(800, 600);

            var color = random.Next() | (0xFF << 24);
            pixelGL.Fill(0 | (0xFF << 24));

            var x0 = random.Next(0, pixelGL.Buffer.Width);
            var y0 = random.Next(0, pixelGL.Buffer.Height);
            var x1 = random.Next(0, pixelGL.Buffer.Width);
            var y1 = random.Next(0, pixelGL.Buffer.Height);
            var x2 = random.Next(0, pixelGL.Buffer.Width);
            var y2 = random.Next(0, pixelGL.Buffer.Height);
            var radius = random.Next(pixelGL.Buffer.Width >> 3, pixelGL.Buffer.Width >> 1);
            color = random.Next() | (0xFF << 24);
            pixelGL.DrawCircle(pixelGL.Buffer.Width >> 1, pixelGL.Buffer.Height >> 1, radius, color);

            pixelGL.Buffer.Save("kernel_circle.png");
        }
    }
}