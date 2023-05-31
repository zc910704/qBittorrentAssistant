using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace qBittorrentAssistant.Resource.Converter
{
    public class SizeConvert : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Int64 size)
            { 
                var readSize = ByteSize.FromBytes(size);
                return readSize.ToString();
            }
            throw new NotImplementedException();
        }
    }
}
