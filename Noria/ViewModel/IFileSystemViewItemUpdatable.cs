namespace Noria.ViewModel
{
    public interface IFileSystemViewItemUpdatable
    {
        void Update(string itemPath, string newItemPath);
    }
}