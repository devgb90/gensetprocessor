using GensetDataProcessor.CalculationEngines;
using GensetDataProcessor.Models;
using GensetDataProcessor.OutputWriters;
using GensetDataProcessor.Parsers;
using Microsoft.Extensions.Logging;

namespace GensetDataProcessor.Watchers
{
    /// <summary>
    /// Watcher for XML files
    /// </summary>
    public class XmlWatcher : IFileWatcher
    {
        private FileSystemWatcher watcher = default!;
        private ILogger logger;
        private IInputParser fileParser;
        private IOutputWriter outputWriter;
        private ICalculationEngine calculationEngine;
        private string outputFolder = string.Empty;

        public XmlWatcher(ILogger<XmlWatcher> logger, IInputParser parser, ICalculationEngine calculationEngine, IOutputWriter outputWriter)
        {
            this.logger = logger;
            this.fileParser = parser;
            this.calculationEngine = calculationEngine;
            this.outputWriter = outputWriter;
        }

        /// <inheritdoc/>
        public void StartWatching(string inputFolder, string outputFolder)
        {
            this.outputFolder = outputFolder;
            this.watcher = new FileSystemWatcher(inputFolder, "*.xml");
            watcher.Created += async (sender, e) => await OnFileCreatedAsync(sender, e);
            watcher.EnableRaisingEvents = true;

            logger.LogInformation($"Watching for XML files in folder: {inputFolder}");
        }

        /// <summary>
        /// Event call back method on create file
        /// Invoked when a file is created in the watched directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnFileCreatedAsync(object sender, FileSystemEventArgs e)
        {
            this.logger.LogInformation($"New file detected : {e.Name}");

            // Read and parse file data
            var data = await this.fileParser.ParseAsync<GenerationInput>(e.FullPath);

            // Perform calculations on the parsed data
            var calculationResult = this.calculationEngine.Calculate(data);

            // Write output to another file in the output folder
            await this.outputWriter.WriteOutputAsync(calculationResult, Path.Combine(outputFolder, $"Output-{e.Name}"));

            this.logger.LogInformation($"Procesing complete for file : {e.Name}");
        }

        /// <inheritdoc/>
        public void StopWatching()
        {
            watcher?.Dispose();
        }
    }
}