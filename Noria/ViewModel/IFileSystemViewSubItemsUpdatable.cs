namespace Noria.ViewModel
{
    public interface IFileSystemViewSubItemsUpdatable
    {
        void AddItem(string itemPath);
        void DeleteItem(string itemPath);
    }
}