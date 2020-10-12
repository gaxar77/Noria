using System.Linq;

namespace Noria.ViewModel
{
    public static class FolderModelExtensions
    {
        public static FolderItemModel GetItemByPath(this FolderModel folder, string itemPath)
        {
            return folder.Items.SingleOrDefault(item => item.ItemPath == itemPath);
        }
    }
}
