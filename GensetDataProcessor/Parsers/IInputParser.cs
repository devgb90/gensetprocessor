namespace GensetDataProcessor.Parsers
{
    /// <summary>
    /// Interface for input file parser
    /// </summary>
    public interface IInputParser
    {
        /// <summary>
        /// Parse file and map data as per type parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        Task<T> ParseAsync<T>(string filepath) where T : class;
    }
}