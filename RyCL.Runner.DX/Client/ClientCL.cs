using RyCL.Graphics;
using SharpDX;
using SharpDX.Direct2D1;

using Bitmap = SharpDX.Direct2D1.Bitmap;
using RectangleF = SharpDX.RectangleF;
using Color = SharpDX.Color;

namespace RyCL.Runner.DX.Client
{
    public class ClientCL : Client2D
    {
        protected static readonly Random _random = new();
        protected static readonly Dictionary<Keys, bool> _keys = new Dictionary<Keys, bool>();
        protected RectangleF _screenRectF;
        protected RasterGL _graphics;
        private int _graphicsStride;
        private Bitmap _frontBuffer;

        protected override void Initialize(ClientConfiguration configuration)
        {
            base.Initialize(configuration);
            var keys = Enum.GetValues(typeof(Keys))
                           .Cast<Keys>()
                           .Select(x => x)
                           .ToArray();
            foreach(var key in keys)
            {
                if (_keys.ContainsKey(key))
                    continue;
                _keys.Add(key, false);
            }
            _screenRectF = new(0, 0, configuration.Width, configuration.Height);
            _graphics = new(configuration.Width, configuration.Height);
            _graphicsStride = _graphics.Buffer.Width << 2;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _frontBuffer = new Bitmap(RenderTarget2D, new Size2(Width, Height), new BitmapProperties(RenderTarget2D.PixelFormat));
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
            _frontBuffer?.Dispose();
            _graphics?.Dispose();
        }

        protected override void BeginDraw()
        {
            _graphics.Draw();
            _frontBuffer.CopyFromMemory(_graphics.Buffer.GetHostBufferRaw(), _graphicsStride);
            base.BeginDraw();
        }
        protected override void Draw(Time time)
        {
            base.Draw(time);
            RenderTarget2D.Clear(Color.Black);
            RenderTarget2D.DrawBitmap(_frontBuffer, _screenRectF, 1f, BitmapInterpolationMode.Linear, _screenRectF);
        }

        protected override void KeyDown(KeyEventArgs e)
        {
            base.KeyDown(e);
            var k = e.KeyCode;
            if (!_keys.ContainsKey(k))
                _keys.Add(k, true);
            _keys[e.KeyCode] = true;
        }
        protected override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);
            var k = e.KeyCode;
            if (!_keys.ContainsKey(k))
                _keys.Add(k, false);
            _keys[e.KeyCode] = false;
        }
    }
}
