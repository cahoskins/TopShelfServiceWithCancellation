using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TopShelfServiceWithCancellation
{
    public class TaskWithCancellation
    {
        private readonly ILogger _logger;
        private readonly TopShelfSettings _settings;

        public TaskWithCancellation(ILoggerFactory logFactory, 
            IOptions<TopShelfSettings> topShelfOptions)
        {
            _settings = topShelfOptions.Value;
            _logger = logFactory.CreateLogger("TaskWithCancellation");
        }
        public Task DoWork(CancellationToken cancelToken)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "tempFile.txt");
            int maxLoops = _settings.MaxLoops;
            using (var writer = File.CreateText(path))
            {
                writer.AutoFlush = true;
                Console.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " Begin");

                for (int i = 0; i < maxLoops; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " Cancellation is requested...");
                        Console.WriteLine("Cancellation is called");
                        Console.WriteLine("Cleaning up");

                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " cleaning up");
                        Thread.Sleep(2000);
                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " done cleaning up");
                        Console.WriteLine("done cleaning up");
                        return Task.FromCanceled(cancelToken);
                    }

                    Console.WriteLine($"processing {i} of {maxLoops}");
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + $" processing {i} of {maxLoops}");
                    Thread.Sleep(500);
                }
                Console.WriteLine("End");
                writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " End");
            }

            return Task.FromResult(maxLoops);
        }
    }
}