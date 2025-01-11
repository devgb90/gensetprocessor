namespace GensetDataProcessor.OutputWriters
{
    internal interface IOutputWriter
    {
        Task WriteOutputAsync<T>(T obj, string filepath) where T : class;
    }
}