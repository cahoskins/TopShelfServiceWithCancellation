using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Topshelf;

namespace TopShelfServiceWithCancellation
{
    public class Program
    {
        public static void Main()
        {
            var outputPath = Environment.CurrentDirectory;
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appSettings.json", optional: false)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .Configure<TopShelfSettings>(options => configuration.GetSection("TopShelf").Bind(options))
                .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var topShelfSettings = serviceProvider.GetService<IOptions<TopShelfSettings>>();
            var stuffToDo = new TaskWithCancellation(loggerFactory, topShelfSettings);
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var settings = topShelfSettings.Value;

            HostFactory.Run(x =>
            {
                x.Service<TopShelfService>(s =>
                {
                    s.ConstructUsing(name => new TopShelfService(stuffToDo, topShelfSettings));
                    s.WhenStarted(tc =>
                    {
                        tc.Start(token);
                    });
                    s.WhenStopped(tc =>
                    {
                        cts.Cancel();
                        Thread.Sleep(5000);
                        tc.Stop(token);
                    });
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription(settings.Description);
                x.SetDisplayName(settings.DisplayName);
                x.SetServiceName(settings.ServiceName);
            });
        }
    }
}
