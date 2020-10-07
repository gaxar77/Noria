using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Shapes = System.Windows.Shapes;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel _viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = _viewModel;

            ViewModelHelper.LoadFolder(_viewModel, @"C:\");
        }
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelHelper.LoadFolder(_viewModel, txtPath.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }
        private void dgrdFolderViewRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FolderViewItemModel item = dgrdFolderView.SelectedItem as FolderViewItemModel;

                if (item != null)
                {
                    ViewModelHelper.LoadFolder(_viewModel, item.FilePath);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModelHelper.LoadParentFolder(_viewModel);
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot navigate to directory.");
            }
        }

        private void dgrdFolderView_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPath.InvalidateProperty(TextBox.TextProperty);
        }
    }

    public class FolderViewItemModel : INotifyPropertyChanged
    {
        private string _fileName;
        private string _filePath;
        private string _fileType;
        private DateTime _modified;
        private DateTime _created;
        private DateTime _lastAccessed;
        private long? _size;
        private bool _isFolder;

        public string FilePath
        {
            get { return _filePath; }

            set
            {
                _filePath = value;

                OnPropertyChanged(nameof(FilePath));
            }
        }
        public string FileName
        {
            get { return _fileName; }

            set
            {
                _fileName = value;

                OnPropertyChanged(nameof(FileName));
            }
        }

        public string FileType
        {
            get { return _fileType; }

            set
            {
                _fileType = value;

                OnPropertyChanged(nameof(FileType));
            }
        }

        public long? Size
        {
            get { return _size; }

            set
            {
                _size = value;

                OnPropertyChanged(nameof(Size));
            }
        }

        public DateTime Modified
        {
            get { return _modified; }

            set
            {
                _modified = value;

                OnPropertyChanged(nameof(Modified));
            }
        }

        public DateTime Created
        {
            get { return _created; }

            set
            {
                _created = value;

                OnPropertyChanged(nameof(Created));
            }
        }

        public DateTime LastAccessed
        {
            get { return _lastAccessed; }

            set
            {
                _lastAccessed = value;

                OnPropertyChanged(nameof(LastAccessed));
            }
        }

        public bool IsFolder
        {
            get { return _isFolder; }

            set
            {
                _isFolder = value;

                OnPropertyChanged(nameof(IsFolder));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
    }
    public class ViewModel : INotifyPropertyChanged
    {
        private string _directoryPath;

        private ObservableCollection<FolderViewItemModel> _folderViewItems;

        public ObservableCollection<FolderViewItemModel> FolderViewItems
        {
            get { return _folderViewItems; }
        }

        public string DirectoryPath
        {
            get { return _directoryPath; }

            set
            {
                _directoryPath = value;

                OnPropertyChanged(nameof(DirectoryPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
        public ViewModel()
        {
            _folderViewItems = new ObservableCollection<FolderViewItemModel>();
        }
    }

    public class ViewModelHelper
    {
        public static void LoadFolder(ViewModel viewModel, string directoryPath)
        {
            viewModel.DirectoryPath = directoryPath;

            viewModel.FolderViewItems.Clear();

            LoadSubDirectories(viewModel, directoryPath);
            LoadFiles(viewModel, directoryPath);
        }
        public static void LoadParentFolder(ViewModel viewModel)
        {
            string parentDirectory = Directory.GetParent(viewModel.DirectoryPath).FullName;

            ViewModelHelper.LoadFolder(viewModel, parentDirectory);
        }
        private static void LoadFiles(ViewModel viewModel, string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            var fileTypeUtility = new FileTypeUtility();

            foreach (string file in files)
            {
                var item = new FolderViewItemModel()
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    Created = File.GetCreationTime(file),
                    Modified = File.GetLastWriteTime(file),
                    LastAccessed = File.GetLastAccessTime(file),
                    Size = FileUtility.GetFileSize(file),
                    IsFolder = false,
                    FileType = fileTypeUtility.GetFileType(file)
                };

                viewModel.FolderViewItems.Add(item);
            }
        }

        private static void LoadSubDirectories(ViewModel viewModel, string directoryPath)
        {
            var subDirectories = Directory.GetDirectories(directoryPath);
            var directoryInfo = new DirectoryInfo(directoryPath);

            foreach (string subDirectory in subDirectories)
            {
                var item = new FolderViewItemModel()
                {
                    FilePath = subDirectory,
                    FileName = Path.GetFileName(subDirectory),
                    Created = Directory.GetCreationTime(subDirectory),
                    Modified = Directory.GetLastWriteTime(subDirectory),
                    LastAccessed = Directory.GetLastAccessTime(subDirectory),
                    Size = null,
                    IsFolder = true
                };

                viewModel.FolderViewItems.Add(item);
            }
        }
    }

    public class FileTypeUtility
    {
        Dictionary<string, string> _fileTypes = new Dictionary<string, string>();

        public FileTypeUtility()
        {

        }

        public string GetFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(extension))
            {
                return String.Empty;
            }

            if (_fileTypes.ContainsKey(extension))
            {
                return _fileTypes[extension];
            }

            using (var extKey = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (extKey != null)
                {
                    var typeKeyName = extKey.GetValue(null, String.Empty);

                    if (typeKeyName != null)
                    {
                        using (var typeKey = Registry.ClassesRoot.OpenSubKey(typeKeyName.ToString()))
                        {
                            if (typeKey != null)
                            {
                                var typeName = typeKey.GetValue(null, String.Empty).ToString();

                                _fileTypes[extension] = typeName;
                                return typeName;
                            }
                        }
                    }
                }
            }

            _fileTypes[extension] = String.Empty;
            
            return String.Empty;
        }
    }

    public class FileUtility
    {
        public static long GetFileSize(string path)
        {
            var fileInfo = new FileInfo(path);

            return fileInfo.Length;
        }
    }

    public class IsFolderToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true
                ? Brushes.Yellow
                : Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
