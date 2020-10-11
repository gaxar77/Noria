﻿//Note: Color distinction between file and folder temporarily removed.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shapes = System.Windows.Shapes;
using Noria.ViewModel;

namespace Noria.UI
{
    //Todo: Move ViewModel and FolderTreeViewModel into one ViewModel class.
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderViewModel _viewModel = new FolderViewModel();
        FolderTreeViewModel _folderTreeViewModel = new FolderTreeViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;

            _viewModel.Navigated += _viewModel_Navigated;

            _folderTreeViewModel.RootFolders.Add(new FolderTreeItemModel(@"C:\"));

            trvFolderTree.ItemsSource = _folderTreeViewModel.RootFolders;

            _viewModel.DirectoryPath = @"C:\";
        }

        private void _viewModel_Navigated(object sender, FolderViewModelNavigationEventArgs e)
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
                    _viewModel.DirectoryPath = item.ItemPath;
                }
                else
                {
                    Process.Start(item.ItemPath);
                }
            }
        }

        private void ScrollToTopOfFolderView()
        {
            var topItem = _viewModel.Folder.Items.FirstOrDefault();

            if (topItem != null)
            {
                dgrdFolderView.ScrollIntoView(topItem);
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
           
            var parentDirectory = Directory.GetParent(_viewModel.DirectoryPath);

            if (parentDirectory != null)
            {
                _viewModel.DirectoryPath = parentDirectory.FullName;
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

                        item.Load();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Unable to rename item.");
                    }

                    _viewModel.TryRefresh();
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

                        _viewModel.Folder.Items.Remove(item);
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
                _viewModel.DirectoryPath = txtPath.Text;
                dgrdFolderView.Focus();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TryNavigateBack();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TryNavigateForward();
        }

        private void DirectoryPathBreadCrumb_BreadCrumbPathSelected(object sender, BreadCrumbPathSelected e)
        {
            _viewModel.DirectoryPath = e.PathComponent.DirectoryPath;
        }

        private void dirPathBreadCrumb_MainPanelMouseDown(object sender, EventArgs e)
        {
            ShowBreadCrumb(false);
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
        }

        private void btnNewFile_Click(object sender, RoutedEventArgs e)
        {
            ShowFeatureNotImplementedMessageBox();
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

            _viewModel.DirectoryPath = folderTreeItem.FolderPath;

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
