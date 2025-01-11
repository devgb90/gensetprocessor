namespace GensetDataProcessor.Watchers
{
    /// <summary>
    /// Interface for watcher implementations
    /// </summary>
    public interface IFileWatcher
    {
        /// <summary>
        /// Sets up watch on a particular folder
        /// </summary>
        /// <param name="inputFolder">Folder to be watched</param>
        /// <param name="outputFolder">Folder to put the outputs</param>
        void StartWatching(string inputFolder, string outputFolder);

        /// <summary>
        /// Disposes the watcher, stops watching the folder
        /// </summary>
        void StopWatching();
    }
}