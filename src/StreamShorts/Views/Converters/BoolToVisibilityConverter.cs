using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using StreamShorts.Projects;

namespace StreamShorts.Views.Converters;
public class BoolToVisibilityConverter : DependencyObject, IValueConverter
{

  public static readonly DependencyProperty TrueVisibilityProperty =
    DependencyProperty.Register(
      "TrueVisibility",
      typeof(Visibility),
      typeof(BoolToVisibilityConverter),
      new PropertyMetadata(Visibility.Visible));

  public static readonly DependencyProperty FalseVisibilityProperty =
    DependencyProperty.Register(
      "FalseVisibility",
      typeof(Visibility),
      typeof(BoolToVisibilityConverter),
      new PropertyMetadata(Visibility.Collapsed));
  public Visibility TrueVisibility
  {
    get { return (Visibility)GetValue(TrueVisibilityProperty); }
    set { SetValue(TrueVisibilityProperty, value); }
  }

  public Visibility FalseVisibility
  {
    get { return (Visibility)GetValue(FalseVisibilityProperty); }
    set { SetValue(FalseVisibilityProperty, value); }
  }
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is bool flag && flag)
    {
      return TrueVisibility;
    }
    return FalseVisibility;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
