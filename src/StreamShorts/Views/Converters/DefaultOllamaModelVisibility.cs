using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using AngleSharp.Text;

namespace StreamShorts.Views.Converters;
public class DefaultOllamaModelVisibility : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {

#pragma warning disable CA1062 // Validate arguments of public methods
    if (values[0] is ObservableCollection<string> list && list.Contains("phi4:latest", StringComparer.OrdinalIgnoreCase))
    {
      return Visibility.Collapsed;
    }
#pragma warning restore CA1062 // Validate arguments of public methods
    return Visibility.Visible;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
