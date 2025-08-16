using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

using StreamShorts.Projects;

namespace StreamShorts.Views.Converters;
public class ProjectFileOpenMediaConverter : IMultiValueConverter
{
  public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values == null || values.Length != 2)
    {
      return null; // Invalid input
    }
    if (values[0] is ProjectFile file && values[1] is MediaElement mediaElement)
    {
      return new ProjectFileMediaElement(file, mediaElement);
    }
    return null;

  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
