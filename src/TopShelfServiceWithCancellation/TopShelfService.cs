using System;
using System.Threading;
using Microsoft.Extensions.Options;

namespace TopShelfServiceWithCancellation
{
    public class TopShelfService : IDisposable
    {
        private Timer _timer;
        private readonly TaskWithCancellation _stuffToDo;
        private readonly TopShelfSettings _settings;

        public TopShelfService(TaskWithCancellation svc,
            IOptions<TopShelfSettings> topShelfSettings)
        {
            _settings = topShelfSettings.Value;
            _stuffToDo = svc;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _timer = new Timer(e => _stuffToDo.DoWork(cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(_settings.TimerExpiresEverySeconds));
        }

        public void Stop(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose() { _timer.Dispose(); }
    }
}