using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;
using Color = SharpDX.Color;

namespace RyCL.Runner.DX.Client
{
    public class Client2D :
        Client3D
    {
        public Factory Factory2D { get; private set; }
        public SharpDX.DirectWrite.Factory FactoryDWrite { get; private set; }
        public RenderTarget RenderTarget2D { get; private set; }
        public SolidColorBrush SceneColorBrush { get; private set; }

        protected override void Initialize(ClientConfiguration configuration)
        {
            base.Initialize(configuration);
            Factory2D = new SharpDX.Direct2D1.Factory();
            using (var surface = BackBuffer.QueryInterface<Surface>())
            {
                RenderTarget2D = new RenderTarget
                (
                    Factory2D, 
                    surface,
                    new RenderTargetProperties
                    (
                        new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)
                    )
                );
            }

            RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
            FactoryDWrite = new SharpDX.DirectWrite.Factory();
            SceneColorBrush = new SolidColorBrush(RenderTarget2D, Color.White);
        }
        protected override void BeginDraw()
        {
            base.BeginDraw();
            RenderTarget2D.BeginDraw();
        }
        protected override void EndDraw()
        {
            RenderTarget2D.EndDraw();
            base.EndDraw();
        }
    }
}
