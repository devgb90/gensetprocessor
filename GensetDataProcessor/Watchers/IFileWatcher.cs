namespace GensetDataProcessor.Watchers
{
    public interface IFileWatcher
    {
        void StartWatching(string inputFolder, string outputFolder);

        void StopWatching();
    }
}