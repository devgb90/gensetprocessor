using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace GensetDataProcessor.Parsers
{
    internal class XmlInputParser : IInputParser
    {
        private ILogger logger;

        public XmlInputParser(ILogger<XmlInputParser> logger)
        {
            this.logger = logger;
        }

        public async Task<T> ParseAsync<T>(string filepath) where T : class
        {
            return await Task.Run(() =>
            {
                this.logger.LogInformation("Reading file content...");
                using var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var serializer = new XmlSerializer(typeof(T));
                var content = (T)serializer.Deserialize(stream)!;
                this.logger.LogInformation("Completed!");
                return content;
            });
        }
    }
}