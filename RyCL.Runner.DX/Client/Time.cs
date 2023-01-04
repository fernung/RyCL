using System.Diagnostics;

namespace RyCL.Runner.DX.Client
{
    public sealed class Time
    {
        private Stopwatch _timer;
        private double _lastUpdate;

        public double ElapsedTime =>
            _timer.ElapsedMilliseconds * 0.001;
        public double TotalSeconds =>
            _timer.Elapsed.TotalSeconds;

        public Time()
        {
            _timer = new Stopwatch();
        }

        public void Start()
        {
            _timer.Start();
            _lastUpdate = 0;
        }
        public void Stop() =>
            _timer.Stop();
        public double Update()
        {
            double now = ElapsedTime;
            double updateTime = now - _lastUpdate;
            _lastUpdate = now;
            return updateTime;
        }
    }
}
