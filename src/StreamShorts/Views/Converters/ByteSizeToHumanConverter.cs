using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Humanizer.Bytes;

namespace StreamShorts.Views.Converters;
public class ByteSizeToHumanConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is long byteSize)
    {
      ByteSize size = ByteSize.FromBytes(byteSize);
#pragma warning disable CA1305 // Specify IFormatProvider
      return size.ToString("0.## mb");
#pragma warning restore CA1305 // Specify IFormatProvider
    }
    return string.Empty;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
