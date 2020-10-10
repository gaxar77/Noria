using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApp1
{
    //Todo: Add SubFolder Drop Downs
    //Todo: Add Horizontal Scrolling
    //Todo: Change properties to dependency properties.

    /// <summary>
    /// Interaction logic for DirectoryPathBreadCrumb.xaml
    /// </summary>
    public partial class DirectoryPathBreadCrumb : UserControl, INotifyPropertyChanged
    {
        public DirectoryPathBreadCrumb()
        {
            InitializeComponent();
        }

        private string _directoryPath;

        public string DirectoryPath
        {
            get 
            { 
                return _directoryPath; 
            }

            set
            {
                _directoryPath = value;

                _pathComponents = GetPathComponents(DirectoryPath);

                CreateBreadCrumbButtons();

                InvokePropertyChanged(nameof(DirectoryPath));
            }
        }

        public event EventHandler<BreadCrumbPathSelected> BreadCrumbPathSelected;

        private BreadCrumbPathComponent[] _pathComponents;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MainPanelMouseDown;

        private void InvokePropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }

        public void CreateBreadCrumbButtons()
        {
            mainPanel.Children.Clear();

            foreach (BreadCrumbPathComponent pathComponent in _pathComponents)
            {
                var breadCrumbButton = new Button()
                {
                    Content = pathComponent.DirectoryName,
                    Padding = new Thickness(5, 5, 5, 5),
                    Background = Brushes.LightBlue,
                    Margin = new Thickness(0, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                breadCrumbButton.Click += (o, e) =>
                    {
                        var args = new BreadCrumbPathSelected(pathComponent);

                        BreadCrumbPathSelected?.Invoke(this, args);
                    };

                mainPanel.Children.Add(breadCrumbButton);
            }
        }

        public BreadCrumbPathComponent[] GetPathComponents(string path)
        {
            var pathComponents = new List<BreadCrumbPathComponent>();

            while (path != null)
            {
                var directoryName = Path.GetFileName(path);
                if (directoryName == String.Empty)
                    directoryName = path;
                
                var pathComponent = new BreadCrumbPathComponent(directoryName, path);

                path = Path.GetDirectoryName(path);

                pathComponents.Add(pathComponent);
            }

            pathComponents.Reverse();

            return pathComponents.ToArray();
        }

        private void mainPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPanelMouseDown?.Invoke(this, new EventArgs());
        }
    }
    public class BreadCrumbPathComponent
    {
        public string DirectoryName { get; private set; }
        public string DirectoryPath { get; private set; }
        public BreadCrumbPathComponent(string directoryName, string directoryPath)
        {
            DirectoryName = directoryName;
            DirectoryPath = directoryPath;
        }
    }

    public class BreadCrumbPathSelected
    {
        public BreadCrumbPathComponent PathComponent { get; private set; }
        public BreadCrumbPathSelected(BreadCrumbPathComponent pathComponent)
        {
            PathComponent = pathComponent;
        }
    }
}
