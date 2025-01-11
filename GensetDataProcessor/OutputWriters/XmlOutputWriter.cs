using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace GensetDataProcessor.OutputWriters
{
    internal class XmlOutputWriter : IOutputWriter
    {
        private ILogger logger;

        public XmlOutputWriter(ILogger<XmlOutputWriter> logger)
        {
            this.logger = logger;
        }

        public async Task WriteOutputAsync<T>(T obj, string filepath) where T : class
        {
            await Task.Run(() =>
            {
                logger.LogInformation("Starting output publish...");
                var serializer = new XmlSerializer(typeof(T));
                using var writer = new StreamWriter(filepath);
                serializer.Serialize(writer, obj);
                writer.FlushAsync();
                logger.LogInformation("Completed!");
            });
        }
    }
}