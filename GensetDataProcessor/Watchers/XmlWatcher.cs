using GensetDataProcessor.CalculationEngines;
using GensetDataProcessor.Models;
using GensetDataProcessor.OutputWriters;
using GensetDataProcessor.Parsers;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor.Watchers
{
    internal class XmlWatcher : IFileWatcher
    {
        private FileSystemWatcher watcher;
        private ILogger logger;
        private IInputParser fileParser;
        private IOutputWriter outputWriter;
        private ICalculationEngine calculationEngine;
        private string outputFolder;

        public XmlWatcher(ILogger<XmlWatcher> logger, IInputParser parser, ICalculationEngine calculationEngine, IOutputWriter outputWriter)
        {
            this.logger = logger;
            this.fileParser = parser;
            this.calculationEngine = calculationEngine;
            this.outputWriter = outputWriter;
        }

        public void StartWatching(string inputFolder, string outputFolder)
        {
            this.outputFolder = outputFolder;
            this.watcher = new FileSystemWatcher(inputFolder, "*.xml");
            watcher.Created += async (sender, e) => await OnFileCreatedAsync(sender, e);
            watcher.EnableRaisingEvents = true;

            logger.LogInformation($"Watching for XML files in folder: {inputFolder}");
        }

        private async Task OnFileCreatedAsync(object sender, FileSystemEventArgs e)
        {
            this.logger.LogInformation($"New file detected : {e.Name}");
            var data = await this.fileParser.ParseAsync<GenerationReport>(e.FullPath);
            var calculationResult = this.calculationEngine.Calculate(data);
            await this.outputWriter.WriteOutputAsync(calculationResult, Path.Combine(outputFolder, $"Output-{e.Name}"));
            this.logger.LogInformation($"Procesing complete for file : {e.Name}");
        }

        public void StopWatching()
        {
            watcher?.Dispose();
        }
    }
}