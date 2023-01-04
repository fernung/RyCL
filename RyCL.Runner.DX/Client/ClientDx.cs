using System;
using System.Windows.Forms;
using SharpDX.Windows;

namespace RyCL.Runner.DX.Client
{
    public abstract class ClientDx : IDisposable
    {
        private bool _disposed;
        private float _frameAccumulator;
        private int _frameCount;
        private readonly Time _clock = new();
        private ClientConfiguration _configuration;

        private FormWindowState _currentFormWindowState;
        private Form _form;

        protected IntPtr DisplayHandle => 
            _form.Handle;
        public ClientConfiguration Configuration => 
            _configuration;
        public float ElapsedTime =>
            (float)_clock.ElapsedTime;
        public int Width =>
            _form.ClientSize.Width;
        public int Height =>
            _form.ClientSize.Height;

        public float FrameDelta { get; private set; }
        public float FramesPerSecond { get; private set; }
        
        ~ClientDx()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(false);
        }

        public void Run() => 
            Run(new ClientConfiguration());
        public void Run(ClientConfiguration configuration)
        {
            _configuration = configuration ?? new ClientConfiguration();
            _form = CreateForm(_configuration);

            Initialize(_configuration);

            bool isFormClosed = false;
            bool isFormResizing = false;

            _form.MouseClick += HandleMouseClick;
            _form.KeyDown += HandleKeyDown;
            _form.KeyUp += HandleKeyUp;
            _form.Resize += (o, e) =>
            {
                if (_form.WindowState != _currentFormWindowState) 
                    HandleResize(o, e);
                _currentFormWindowState = _form.WindowState;
            };
            _form.ResizeBegin += (o, e) => 
            { 
                isFormResizing = true; 
            };
            _form.ResizeEnd += (o, e) =>
            {
                isFormResizing = false;
                HandleResize(o, e);
            };
            _form.Closed += (o, e) => 
            { 
                isFormClosed = true; 
            };

            LoadContent();

            _clock.Start();
            BeginRun();
            RenderLoop.Run(_form, () =>
            {
                if (isFormClosed) 
                    return;

                OnUpdate();
                if (!isFormResizing) 
                    OnRender();
            });

            UnloadContent();
            EndRun();
            Dispose();
        }
        public void Exit() =>
            _form.Close();
        public void Dispose()
        {
            if(!_disposed)
            {
                Dispose(true);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        protected virtual Form CreateForm(ClientConfiguration configuration)
        {
            return new RenderForm(configuration.Title)
            {
                ClientSize = new System.Drawing.Size(configuration.Width, configuration.Height)
            };
        }

        protected abstract void Initialize(ClientConfiguration configuration);
        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }
        protected virtual void Update(Time time) { }
        protected virtual void Draw(Time time) { }
        protected virtual void BeginRun() { }
        protected virtual void EndRun() { }
        protected virtual void BeginDraw() { }
        protected virtual void EndDraw() { }
        protected virtual void MouseClick(MouseEventArgs e) { }
        protected virtual void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Exit();
        }
        protected virtual void KeyUp(KeyEventArgs e) { }
        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                _form?.Dispose();
            }
        }

        private void OnUpdate()
        {
            FrameDelta = (float)_clock.Update();
            Update(_clock);
        }
        private void OnRender()
        {
            _frameAccumulator += FrameDelta;
            ++_frameCount;
            if (_frameAccumulator >= 1.0f)
            {
                FramesPerSecond = _frameCount / _frameAccumulator;
                _form.Text = _configuration.Title + " - FPS: " + FramesPerSecond;
                _frameAccumulator = 0.0f;
                _frameCount = 0;
            }

            BeginDraw();
            Draw(_clock);
            EndDraw();
        }
        private void HandleMouseClick(object sender, MouseEventArgs e) => 
            MouseClick(e);
        private void HandleKeyDown(object sender, KeyEventArgs e) => 
            KeyDown(e);
        private void HandleKeyUp(object sender, KeyEventArgs e) => 
            KeyUp(e);
        private void HandleResize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized) 
                return;
        }
    }
}
