using GensetDataProcessor.CalculationEngines;
using GensetDataProcessor.OutputWriters;
using GensetDataProcessor.Parsers;
using GensetDataProcessor.Watchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
           .ConfigureServices((context, services) =>
           {
               services.AddTransient<IFileWatcher, XmlWatcher>();
               services.AddTransient<IInputParser, XmlInputParser>();
               services.AddTransient<ICalculationEngine, GeneratorAnalyticsEngine>();
               services.AddTransient<IOutputWriter, XmlOutputWriter>();
               services.AddSingleton<ReferenceDataProvider>();
               services.AddSingleton<App>();
           })
           .ConfigureLogging(logging =>
           {
               logging.ClearProviders();
               logging.AddConsole();
           })
           .Build();

            try
            {
                var app = host.Services.GetRequiredService<App>();
                Console.WriteLine("Press ESC to stop");
                app.Start();
                do
                {
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                app.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR - {ex.Message}");
                Console.WriteLine("Shutting down ...");
            }
        }
    }
}