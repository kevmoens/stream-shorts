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

using StreamShorts.MVVM;
using StreamShorts.ViewModels;

namespace StreamShorts.Views;
/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : UserControl, IPage
{
  public Settings(SettingsViewModel viewModel) : this()
  {
    DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
  }
  public Settings()
  {
    InitializeComponent();
  }

  public string PageKey => "Settings";
}
