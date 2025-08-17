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

using StreamShorts.MVVM.MVVM;
using StreamShorts.ViewModels;

namespace StreamShorts.Views;
/// <summary>
/// Interaction logic for InstallOllama.xaml
/// </summary>
public partial class InstallOllama : UserControl, IPage
{
  public InstallOllama(InstallOllamaViewModel viewModel) : this()
  {
    DataContext = viewModel;
  }
  public InstallOllama()
  {
    InitializeComponent();
  }

  public string PageKey => "InstallOllama";
}
