using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace RyCL.Runner.DX.Client
{
    public class Client3D :
        ClientDx
    {
        SharpDX.Direct3D11.Device _device;
        SwapChain _swapChain;
        Texture2D _backBuffer;
        RenderTargetView _backBufferView;

        public SharpDX.Direct3D11.Device Device => 
            _device;
        public Texture2D BackBuffer => 
            _backBuffer;
        public RenderTargetView BackBufferView => 
            _backBufferView;

        protected override void Initialize(ClientConfiguration configuration)
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(configuration.Width, configuration.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = DisplayHandle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            SharpDX.Direct3D11.Device.CreateWithSwapChain
            (
                DriverType.Hardware, 
                DeviceCreationFlags.BgraSupport, 
                new[] { FeatureLevel.Level_10_0 }, 
                desc, 
                out _device, 
                out _swapChain
            );

            Factory factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(DisplayHandle, WindowAssociationFlags.IgnoreAll);

            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _backBufferView = new RenderTargetView(_device, _backBuffer);
        }
        protected override void BeginDraw()
        {
            base.BeginDraw();
            var viewport = new Viewport(0, 0, Configuration.Width, Configuration.Height);
            Device.ImmediateContext.Rasterizer.SetViewport(viewport);
            Device.ImmediateContext.OutputMerger.SetTargets(_backBufferView);
        }
        protected override void EndDraw()
        {
            _swapChain.Present(Configuration.WaitVBlank ? 1 : 0, PresentFlags.None);
        }
    }
}
