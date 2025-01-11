using System.Xml.Linq;

namespace GensetDataProcessor.Parsers
{
    internal class XDocumentParser : IInputParser
    {
        public async Task<T> ParseAsync<T>(string filepath) where T : class
        {
            using FileStream stream = File.OpenRead(filepath);
            var xmlDoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            return null;
        }
    }
}