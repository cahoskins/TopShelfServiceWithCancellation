using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TopShelfServiceWithCancellation
{
    public class TaskWithCancellation
    {
        public Task DoWork(CancellationToken cancelToken, IProgress<string> progress)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "tempFile.txt");
            int maxLoops = 5;
            using (var writer = File.CreateText(path))
            {
                writer.AutoFlush = true;
                progress.Report("\r\nBegin");
                writer.WriteLine("\r\nBegin");
                writer.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());

                for (int i = 0; i < maxLoops; i++)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " Cancellation is requested...");
                        progress.Report("Cancellation is called");
                        progress.Report("Cleaning up");

                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " cleaning up");
                        Thread.Sleep(2000);
                        writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " done cleaning up");
                        progress.Report("done cleaning up");
                        return Task.FromCanceled(cancelToken);
                    }

                    progress.Report($"processing {i} of {maxLoops}");
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + $" processing {i} of {maxLoops}");
                    Thread.Sleep(500);
                }
                progress.Report("End");
                writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " End");
            }

            return Task.FromResult(maxLoops);
        }
    }
}