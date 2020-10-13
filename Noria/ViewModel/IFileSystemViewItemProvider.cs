namespace Noria.ViewModel
{
    //Todo: Change suffix provider to something else. May be inappropriate use of the term.
    public interface IFileSystemViewItemProvider
    {
        IFileSystemViewItem GetFileSystemViewItem(string path);
    }
}