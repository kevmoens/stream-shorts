using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace StreamShorts.Views.Converters;
public static class TextBoxHelper
{
  public static string GetPlaceholder(DependencyObject obj)
  {
    if (obj == null)
    {
      return string.Empty;
    }
    return (string)obj.GetValue(PlaceholderProperty);
  }

  public static void SetPlaceholder(DependencyObject obj, string value)
  {
    if (obj != null)
    {
      obj.SetValue(PlaceholderProperty, value);
    }
  }

  public static readonly DependencyProperty PlaceholderProperty =
      DependencyProperty.RegisterAttached(
          "Placeholder",
          typeof(string),
          typeof(TextBoxHelper),
          new FrameworkPropertyMetadata(
              defaultValue: null,
              propertyChangedCallback: OnPlaceholderChanged)
          );
  private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is TextBox textBoxControl)
    {
      if (!textBoxControl.IsLoaded)
      {
        // Ensure that the events are not added multiple times
        textBoxControl.Loaded -= TextBoxControl_Loaded;
        textBoxControl.Loaded += TextBoxControl_Loaded;
      }

      textBoxControl.TextChanged -= TextBoxControl_TextChanged;
      textBoxControl.TextChanged += TextBoxControl_TextChanged;

      textBoxControl.IsVisibleChanged -= TextBoxControl_IsVisibleChanged;
      textBoxControl.IsVisibleChanged += TextBoxControl_IsVisibleChanged;

      // If the adorner exists, invalidate it to draw the current text
      if (GetOrCreateAdorner(textBoxControl, out PlaceholderAdorner? adorner) && adorner != null)
        adorner.InvalidateVisual();
    }
  }
  private static void TextBoxControl_Loaded(object sender, RoutedEventArgs e)
  {
    if (sender is TextBox textBoxControl)
    {
      textBoxControl.Loaded -= TextBoxControl_Loaded;
      GetOrCreateAdorner(textBoxControl, out _);
    }
  }

  private static void TextBoxControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is TextBox textBoxControl
        && GetOrCreateAdorner(textBoxControl, out PlaceholderAdorner? adorner) && adorner != null)
    {
      // If the control is visible, show the adorner
      if (textBoxControl.IsVisible)
      {
        adorner.Visibility = Visibility.Visible;
        // If the control has text, hide the adorner
        if (textBoxControl.Text.Length > 0)
          adorner.Visibility = Visibility.Collapsed;
      }
      else
      {
        // If the control is not visible, hide the adorner
        adorner.Visibility = Visibility.Collapsed;
      }
    }
  }

  private static void TextBoxControl_TextChanged(object sender, TextChangedEventArgs e)
  {
    if (sender is TextBox textBoxControl
        && GetOrCreateAdorner(textBoxControl, out PlaceholderAdorner? adorner) && adorner != null)
    {
      // Control has text. Hide the adorner.
      if (textBoxControl.Text.Length > 0)
        adorner.Visibility = Visibility.Collapsed;

      // Control has no text. Show the adorner.
      else
        adorner.Visibility = Visibility.Visible;
    }
  }
  private static bool GetOrCreateAdorner(TextBox textBoxControl, out PlaceholderAdorner? adorner)
  {
    // Get the adorner layer
    AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBoxControl);

    // If null, it doesn't exist or the control's template isn't loaded
    if (layer == null)
    {
      adorner = null;
      return false;
    }

    // Layer exists, try to find the adorner
    adorner = layer.GetAdorners(textBoxControl)?.OfType<PlaceholderAdorner>().FirstOrDefault();

    // Adorner never added to control, so add it
    if (adorner == null)
    {
      adorner = new PlaceholderAdorner(textBoxControl);
      layer.Add(adorner);
    }

    return true;
  }
}
