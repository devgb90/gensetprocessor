namespace GensetDataProcessor.OutputWriters
{
    public interface IOutputWriter
    {
        Task WriteOutputAsync<T>(T obj, string filepath) where T : class;
    }
}