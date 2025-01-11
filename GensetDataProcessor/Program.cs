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
        private static void Main(string[] args)
        {
            // Configure DI container
            var host = Host.CreateDefaultBuilder(args)
           .ConfigureServices((context, services) =>
           {
               services.AddTransient<IFileWatcher, XmlWatcher>();
               services.AddTransient<IInputParser, XmlInputParser>();
               services.AddTransient<ICalculationEngine, GeneratorAnalyticsEngine>();
               services.AddTransient<IOutputWriter, XmlOutputWriter>();
               services.AddSingleton<ReferenceDataProvider>();
               // App is the main application
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
                // Start the application
                app.Start();
                do
                {
                    // Keep checking for ESC
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

                // When ESC is pressed, stop the app
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