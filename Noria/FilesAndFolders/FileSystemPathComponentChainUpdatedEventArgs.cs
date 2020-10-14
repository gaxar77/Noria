namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentChainUpdatedEventArgs
    {
        public string Path { get; private set; }

        public FileSystemPathComponentChainUpdatedEventArgs(string path)
        {
            Path = path;
        }
    }
}