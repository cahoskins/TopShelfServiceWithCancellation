using System;
using System.Threading;

namespace TopShelfServiceWithCancellation
{
    public class TopShelfService
    {
        private Timer _timer;
        private readonly TaskWithCancellation _stuffToDo;

        public TopShelfService()
        {
            _stuffToDo = new TaskWithCancellation();
        }
        public void Start(CancellationToken cancelToken, IProgress<string> progress)
        {
            _timer = new Timer(e => _stuffToDo.DoWork(cancelToken, progress), null, TimeSpan.Zero, TimeSpan.FromSeconds(17));
        }
        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }
    }
}