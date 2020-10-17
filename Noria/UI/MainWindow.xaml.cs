using Noria.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Noria.Misc;
using System.Windows.Navigation;

namespace Noria.UI
{
    public class NewItemNameGenerator
    {
        public int _index = 0;
        public string _baseName;

        public NewItemNameGenerator(string baseName)
        {
            _baseName = baseName;
        }
        public string GenerateName()
        {
            _index++;

            if (_index == 1)
            {
                return _baseName;
            }

            return $"{_baseName} {_index}";
        }
    }

  
    //Todo: Fix problem where deleting the parent or ancestors of the folder in the current view causes nothing to happen, but creates an invalid application state.
    //Todo: Implement cut, copy, and paste.
        //Copy and paste partially implemented.
        //Todo: Implement file copy progress dialog.
        //Todo: Implement initial emptiness of clipboard.
        //Todo: Implement overwrite confirm messagebox.
        //Todo: Support copying of folders, not just files.
    //Todo: Implement display of file and folder icons in the folder view.
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        //FolderViewModelUpdaterThroughWatchingFolder _watcherAndUpdater;

        private FileSystemItemClipboard _itemClipboard
            = new FileSystemItemClipboard();
        public MainWindow()
        {
            InitializeComponent();

            LoadViewModel();
            LoadTreeViewRootFolderSubFolders();

            _viewModel.FolderViewModel.TryRefresh();
        }

        private void LoadTreeViewRootFolderSubFolders()
        {
            var rootFolders = _viewModel
                .FolderTreeViewModel
                .RootFolders;

            foreach (FolderTreeItemModel rootFolder in rootFolders)
            {
                rootFolder.LoadSubFolders();
            }
        }

        private void LoadViewModel()
        {
            var viewModelBuilder = new MainWindowViewModelBuilder();
            viewModelBuilder.BuildAll();

            _viewModel = viewModelBuilder.ViewModel;
            DataContext = _viewModel;

            _viewModel.FolderViewModel.Navigated += _folderViewModel_Navigated;
        }

        private void _folderViewModel_Navigated(object sender, FolderViewModelNavigationEventArgs e)
        {
            if (e.NewFolder is InaccessibleFolderModel)
            {
                MessageBox.Show("Unable to access this folder.");
            }

            ScrollToTopOfFolderView();
        }

        private void dgrdFolderViewRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedFolderViewItem();
        }


        private void OpenSelectedFolderViewItem()
        {
            FolderItemModel item = dgrdFolderView.SelectedItem as FolderItemModel;

            if (item != null)
            {
                if (item is FolderFolderItemModel)
                {
                    _viewModel.FolderViewModel.DirectoryPath = item.ItemPath;
                }
                else
                {
                    Process.Start(item.ItemPath);
                }
            }
        }

        private void ScrollToTopOfFolderView()
        {
            var topItem = _viewModel.FolderViewModel.Folder.Items.FirstOrDefault();

            if (topItem != null)
            {
                dgrdFolderView.ScrollIntoView(topItem);
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {

            var parentDirectory = Directory.GetParent(_viewModel.FolderViewModel.DirectoryPath);

            if (parentDirectory != null)
            {
                _viewModel.FolderViewModel.DirectoryPath = parentDirectory.FullName;
            }
        }

        private void dgrdFolderView_GotFocus(object sender, RoutedEventArgs e)
        {
            ShowBreadCrumb(true);
        }

        private void dgrdFolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowFeatureNotImplementedMessageBox()
        {
            MessageBox.Show("Feature not yet implemented.");
        }
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedFolderViewItem();
        }

        //Todo: Refactor
        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            RenameSelectedItem();
        }

        private void RenameSelectedItem()
        {
            if (dgrdFolderView.SelectedItem is FolderItemModel item)
            {
                var renameFileDialog = new RenameFileDialog();
                renameFileDialog.ItemPath = item.ItemPath;
                renameFileDialog.ItemOldName = item.ItemName;
                renameFileDialog.ItemNewName = item.ItemName;

                renameFileDialog.ShowDialog();

                if (renameFileDialog.UserConfirmedRename)
                {
                    try
                    {
                        var parentDir = Path.GetDirectoryName(item.ItemPath);
                        var newItemPath = Path.Combine(parentDir, renameFileDialog.ItemNewName);
                        if (item is FolderFolderItemModel)
                        {
                            Directory.Move(item.ItemPath, newItemPath);
                        }
                        else
                        {
                            File.Move(item.ItemPath, newItemPath);
                        }

                        //item.Load();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to rename item.");
                    }

                    //if (!HasFolderViewModelFileSystemWatcher())
                    //    _viewModel.FolderViewModel.TryRefresh();
                }
            }
        }

        //Todo: Implement Recycle Bin Storage
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdFolderView.SelectedItem is FolderItemModel item)
            {
                var result = MessageBox.Show("Are you sure you want to delete this item? You will not be able to recover it.",
                    null, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (item is FolderFolderItemModel)
                        {
                            Directory.Delete(item.ItemPath, true);
                        }
                        else
                        {
                            File.Delete(item.ItemPath);
                        }

                        //if (!HasFolderViewModelFileSystemWatcher())
                        //{
                        //    _viewModel.FolderViewModel.Folder.Items.Remove(item);
                        //}
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to delete item.");
                    }
                }
            }
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            var fileSystemItem = dgrdFolderView.SelectedItem as FolderItemModel;

            if (fileSystemItem != null)
            {
                var clipboardItem = new FileSystemItemClipboardItem()
                {
                    ItemPath = fileSystemItem.ItemPath,
                    Operation = FileSystemItemClipboardStorageOperation.Copy
                };

                _itemClipboard.Store(clipboardItem);
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            var clipboardItem = _itemClipboard.Retrieve();

            if (clipboardItem.Operation == FileSystemItemClipboardStorageOperation.Copy)
            {
                if (File.Exists(clipboardItem.ItemPath) &&
                    !(_viewModel.FolderViewModel.Folder is InaccessibleFolderModel))
                {
                    try
                    {
                        var itemName = Path.GetFileName(clipboardItem.ItemPath);
                        var destinationPath = Path.Combine(
                            _viewModel.FolderViewModel.DirectoryPath,
                            itemName);

                        File.Copy(clipboardItem.ItemPath, destinationPath, false);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to copy item.");
                    }
                }
            }
        }

        private void btnProperties_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        //Todo: Add Path validation
        private void txtPath_LostFocus(object sender, RoutedEventArgs e)
        {
            txtPath.InvalidateProperty(TextBox.TextProperty);

            ShowBreadCrumb(true);
        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.FolderViewModel.DirectoryPath = txtPath.Text;
                dgrdFolderView.Focus();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FolderViewModel.TryNavigateBack();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FolderViewModel.TryNavigateForward();
        }

        private void DirectoryPathBreadCrumb_BreadCrumbPathSelected(object sender, BreadCrumbPathSelectedEventArgs e)
        {
            _viewModel.FolderViewModel.DirectoryPath = e.DirectoryPath;
        }

        private void dirPathBreadCrumb_MainPanelMouseDown(object sender, EventArgs e)
        {
            ShowBreadCrumb(false);
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nameGen = new NewItemNameGenerator("New Folder");

                while(true)
                {
                    var name = nameGen.GenerateName();
                    var path = Path.Combine(
                        _viewModel.FolderViewModel.DirectoryPath,
                        name);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);

                        AddAndSelectItem(path);
                        RenameSelectedItem();
                        break;
                    }
                };
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to create folder.");
            }
        }

        //Todo: Allow user to select file type and generate it according to rules in the registry.
        private void btnNewFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nameGen = new NewItemNameGenerator("New File");

                while (true)
                {
                    var name = nameGen.GenerateName();
                    var path = Path.Combine(
                        _viewModel.FolderViewModel.DirectoryPath,
                        name);

                    if (!File.Exists(path))
                    {
                        File.Create(path);

                        AddAndSelectItem(path);
                        RenameSelectedItem();

                        break;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to create file.");
            }
        }

        private void AddAndSelectItem(string path)
        {
            _viewModel
                .FolderViewModel
                .Folder
                .AddItem(path);

            var item = _viewModel
                .FolderViewModel
                .Folder
                .GetItemByPath(path);

            dgrdFolderView.SelectedItem = item;
            dgrdFolderView.ScrollIntoView(item);
        }

        private void trvFolderTree_Item_Expanded(object sender, RoutedEventArgs e)
        {
            if (((TreeViewItem)sender).DataContext is FolderTreeItemModel item)
            {
                foreach (FolderTreeItemModel subFolder in item.SubFolders)
                {
                    if (!subFolder.AreSubFoldersLoaded)
                        subFolder.LoadSubFolders();
                }

                item.IsExpandedInUI = true;
            }

            e.Handled = true;
        }
        private void trvFolderTree_Item_Selected(object sender, RoutedEventArgs e)
        {
        }

        private void ShowBreadCrumb(bool show)
        {
            dirPathBreadCrumb.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            txtPath.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!dirPathBreadCrumb.IsMouseOver)
            {
                ShowBreadCrumb(true);
            }
        }

        private void trvFolderTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowBreadCrumb(true);
        }

        private void dgrdFolderView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowBreadCrumb(true);
        }

        private void dgrdFolderView_ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            ShowBreadCrumb(true);
        }

        private void trvFolderTree_Item_Collapsed(object sender, RoutedEventArgs e)
        {
            if (((TreeViewItem)sender).DataContext is FolderTreeItemModel item)
            {
                item.IsExpandedInUI = false;
            }

            e.Handled = true;
        }

        private void trvFolderTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (trvFolderTree.SelectedItem is FolderTreeItemModel item)
            {
                _viewModel.FolderViewModel.DirectoryPath = item.FolderPath;
            }
        }
    }
}
