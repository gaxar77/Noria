namespace Noria.ViewModel
{
    public interface IFileSystemItemUpdatable
    {
        void Update(string itemPath, string newItemPath);
    }
}