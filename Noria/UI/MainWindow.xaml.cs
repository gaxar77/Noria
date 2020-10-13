using Noria.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    //Todo: Implement real-time updating of Folder Tree View.
    //Todo: Refactor Code, moving Folder File System Watcher into the FolderViewModel class.
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;
        //FolderViewModelUpdaterThroughWatchingFolder _watcherAndUpdater;
        public MainWindow()
        {
            InitializeComponent();

            LoadViewModel();

            _viewModel.FolderViewModel.TryRefresh();
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
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
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

        private void DirectoryPathBreadCrumb_BreadCrumbPathSelected(object sender, BreadCrumbPathSelected e)
        {
            _viewModel.FolderViewModel.DirectoryPath = e.PathComponent.DirectoryPath;
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
            var item = (TreeViewItem)sender;

            var folderTreeItem = (FolderTreeItemModel)item.Header;

            folderTreeItem.LoadSubFolders();

            e.Handled = true;
        }

        private void trvFolderTree_Item_Selected(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            var folderTreeItem = (FolderTreeItemModel)item.Header;

            _viewModel.FolderViewModel.DirectoryPath = folderTreeItem.FolderPath;

            e.Handled = true;
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
    }
}
