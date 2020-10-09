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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RenameFileDialog : Window, INotifyPropertyChanged
    {
        public RenameFileDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedRename = true;
            Close();
        }

        private string _newName;
        private string _oldName;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool UserConfirmedRename { get; private set; }

        public string ItemNewName
        {
            get { return _newName; }

            set
            {
                _newName = value;

                InvokePropertyChanged(nameof(ItemNewName));
            }
        }

        public string ItemOldName
        {
            get { return _oldName; }

            set
            {
                _oldName = value;

                InvokePropertyChanged(nameof(ItemOldName));
            }
        }

        private void InvokePropertyChanged(string propertyName)
        {
            var args = new PropertyChangedEventArgs(propertyName);

            PropertyChanged?.Invoke(this, args);
        }
    }
}
