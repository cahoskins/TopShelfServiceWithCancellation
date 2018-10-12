using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace TopShelfServiceWithCancellation
{
    public class Program
    {
        public static void Main()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            IProgress<string> progressHandler = new Progress<string>(Console.WriteLine);

            HostFactory.Run(x =>
            {
                x.Service<TopShelfService>(s =>
                {
                    s.ConstructUsing(name => new TopShelfService());
                    s.WhenStarted(tc =>
                    {
                        tc.Start(token, progressHandler);
                    });
                    s.WhenStopped(tc =>
                    {
                        cts.Cancel();
                            Thread.Sleep(5000);
                        tc.Stop();
                    });
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription("Sample TopShelf Service with Cancellation");
                x.SetDisplayName("TopShelfServiceWithCancellation");
                x.SetServiceName("TopShelfService");
            });
        }
    }
}
