using GensetDataProcessor.Watchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor
{
    public class App
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private readonly string inputFolder;
        private readonly string outputFolder;
        private readonly string refDataPath;
        private readonly IFileWatcher watcherService;

        public App(IConfiguration configuration, ILogger<App> logger, IFileWatcher watcherService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.watcherService = watcherService;
            this.inputFolder = configuration.GetValue<string>("SourceDirectory")!;
            this.outputFolder = configuration.GetValue<string>("OutputDirectory")!;
            this.refDataPath = configuration.GetValue<string>("ReferenceDataFilePath")!;
        }

        public void Start()
        {
            try
            {
                this.logger.LogInformation("Application starting up...");
                ValidateDirectories();
                this.watcherService.StartWatching(this.inputFolder, this.outputFolder);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, message: ex.Message);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                this.logger.LogInformation("Stopping application...");
                this.watcherService.StopWatching();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, message: ex.Message);
                throw;
            }
        }

        private void ValidateDirectories()
        {
            if (string.IsNullOrEmpty(inputFolder) || !Directory.Exists(inputFolder))
            {
                throw new ArgumentException("Source directory is invalid. Please check configuration.");
            }
            this.logger.LogInformation($"Configured input directory : {this.inputFolder}");

            if (string.IsNullOrEmpty(outputFolder))
            {
                throw new ArgumentException("Output directory is not specified. Please check configuration");
            }
            this.logger.LogInformation($"Configured output directory : {this.outputFolder}");
            if (!Directory.Exists(outputFolder))
            {
                this.logger.LogInformation($"Output directory does not exist. Creating directory {this.outputFolder}");
                Directory.CreateDirectory(outputFolder);
            }
        }
    }
}