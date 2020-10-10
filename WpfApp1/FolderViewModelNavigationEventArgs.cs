using System;

namespace WpfApp1
{
    public class FolderViewModelNavigationEventArgs : EventArgs
    {
        public ImmutableFolder PreviousFolder { get; private set; }
        public ImmutableFolder NewFolder { get; private set; }

        public FolderViewModelNavigationEventArgs(ImmutableFolder previousFolder, ImmutableFolder newFolder)
        {
            PreviousFolder = previousFolder;
            NewFolder = newFolder;
        }
    }
}
