using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using StreamShorts.Projects;

namespace StreamShorts.Views.Converters;
public class FilesProcessButtonVisibilityConverter : IMultiValueConverter
{
  public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
#pragma warning disable CA1062 // Validate arguments of public methods
    if (values.Length < 2)
    {
      return Visibility.Collapsed;
    }
#pragma warning restore CA1062 // Validate arguments of public methods

    if (values[1] != null && values[1] is bool isProcessing && isProcessing)
    {
      return Visibility.Collapsed;
    }
    if (values[0] != null && values[0] is ObservableCollection<ProjectFile> projectFiles && projectFiles.Count == 0)
    {
      return Visibility.Visible;
    }
    return Visibility.Collapsed;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
