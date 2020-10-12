namespace Noria.ViewModel
{
    public interface IFileSystemSubItemsUpdatable
    {
        void AddItem(string itemPath);
        void DeleteItem(string itemPath);
    }
}