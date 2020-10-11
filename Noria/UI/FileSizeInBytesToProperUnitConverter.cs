using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace Noria.UI
{
    class FileSizeInBytesToProperUnitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int maxDecimalDigitsCount = parameter != null ? (int)parameter : 2;
            
            if (value == null)
                return null;

            double sizeInBytes = (long)value;
            double sizeInKillobytes = Math.Round(sizeInBytes / 1024, 
                maxDecimalDigitsCount);
            double sizeInMegabytes = Math.Round(sizeInKillobytes / 1024,
                maxDecimalDigitsCount);
            double sizeInGigabytes = Math.Round(sizeInMegabytes / 1024,
                maxDecimalDigitsCount);
            double sizeInTerabytes = Math.Round(sizeInGigabytes / 1024,
                maxDecimalDigitsCount);

            if (sizeInTerabytes >= 1)
                return $"{sizeInTerabytes} TB";

            if (sizeInGigabytes >= 1)
                return $"{sizeInGigabytes} GB";
           
            if (sizeInMegabytes >= 1)
                return $"{sizeInMegabytes} MB";

            if (sizeInKillobytes >= 1)
                return $"{sizeInKillobytes} KB";

            return $"{sizeInBytes} bytes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
