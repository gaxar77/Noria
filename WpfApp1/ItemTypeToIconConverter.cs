using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace WpfApp1
{
    public class ItemTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ImmutableFolderItemType)value == ImmutableFolderItemType.Folder
                ? Brushes.Yellow
                : Brushes.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
