using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using StreamShortsMVVM.Interfaces;

namespace StreamShorts.UI;
public class SettingsCanSave : ISettingsCanSave
{

#pragma warning disable CA1822 // Mark members as static
  public bool CanSave()
#pragma warning restore CA1822 // Mark members as static
  {
    // This assumes you only have one main window with the settings controls
    foreach (Window window in System.Windows.Application.Current.Windows)
    {
      if (HasValidationError(window))
        return false;
    }
    return true;
  }

  private static bool HasValidationError(DependencyObject obj)
  {
    if (System.Windows.Controls.Validation.GetHasError(obj))
      return true;

    for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
    {
      var child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
      if (HasValidationError(child))
        return true;
    }
    return false;
  }
}
