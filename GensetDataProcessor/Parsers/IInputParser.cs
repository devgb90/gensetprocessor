namespace GensetDataProcessor.Parsers
{
    public interface IInputParser
    {
        Task<T> ParseAsync<T>(string filepath) where T : class;
    }
}