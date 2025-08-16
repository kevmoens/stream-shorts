using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using StreamShorts.Library.Analysis;
using StreamShorts.Projects;

namespace StreamShorts.Views.Converters;
public class LLMProviderVisibilityConverter : DependencyObject, IValueConverter
{
  public static readonly DependencyProperty TargetProviderProperty =
      DependencyProperty.Register(
          "TargetProvider",
          typeof(LLMProvider),
          typeof(LLMProviderVisibilityConverter),
          new PropertyMetadata(LLMProvider.ChatGpt));
	public static readonly DependencyProperty TrueVisibilityProperty =
		DependencyProperty.Register(
			"TrueVisibility",
			typeof(Visibility),
			typeof(LLMProviderVisibilityConverter),
			new PropertyMetadata(Visibility.Visible));

	public static readonly DependencyProperty FalseVisibilityProperty =
		DependencyProperty.Register(
			"FalseVisibility",
			typeof(Visibility),
			typeof(LLMProviderVisibilityConverter),
			new PropertyMetadata(Visibility.Collapsed));
  public LLMProvider TargetProvider
    {
    get { return (LLMProvider)GetValue(TargetProviderProperty); }
    set { SetValue(TargetProviderProperty, value); }
  }

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
    if (value is LLMProvider provider)
    {
      if (provider == TargetProvider)
      {
        return TrueVisibility;
      }
      else
      {
        return FalseVisibility;
      }
    }
    return FalseVisibility; // Default to FalseVisibility if value is not of type LLMProvider
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}
