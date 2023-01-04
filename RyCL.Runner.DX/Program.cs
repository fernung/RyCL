using RyCL.Runner.DX.Demos;

namespace RyCL.Runner.DX
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var client = new ShapesDemo();
            client.Run();
        }
    }
}