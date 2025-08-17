using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.UI;
/// <summary>
/// Interaction logic for UIMediaElement.xaml
/// </summary>
public partial class UIMediaElement : UserControl, IMediaElement
{
  public UIMediaElement()
  {
    InitializeComponent();
  }
  public DependencyProperty SourceProperty { get; set; } = DependencyProperty.Register(
    "Source", typeof(Uri), typeof(UIMediaElement), new PropertyMetadata(null, OnSourceChanged));

  private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is UIMediaElement mediaElement)
    {
      if (e.NewValue is Uri uri)
      {
        mediaElement.mediaElement.Source = uri;
        mediaElement.mediaElement.Play();
      }
      else
      {
        mediaElement.mediaElement.Source = null;
      }
    }
  }

  public Uri? Source
  {
    get => (Uri?)GetValue(SourceProperty);
    set => SetValue(SourceProperty, value);
  }
  public void Pause()
  {
    mediaElement.Pause();
  }

  public void Play()
  {
    mediaElement.Play();
  }

  public void Stop()
  {
    mediaElement.Stop();
  }
}
