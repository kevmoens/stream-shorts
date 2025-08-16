using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using StreamShorts.Library;
using StreamShorts.Projects;

namespace StreamShorts.Views.Converters;
public class ProjectPreviewPathConverter : IMultiValueConverter
{
  public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
  {
    if (values != null && values.Length == 2 && values[0] is string preview && values[1] is string projectName)
    {
      // Combine the preview and project name to create the image source
      string imagePath = Path.Combine(WorkingDirectory.Current, projectName, preview);
      if (File.Exists(imagePath))
      {
        using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = stream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensures the stream can be closed after loading
        bitmap.EndInit();
        bitmap.Freeze(); // Optional: makes it cross-thread accessible
        return bitmap;
      }
    }
    return null;
  }

  public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
