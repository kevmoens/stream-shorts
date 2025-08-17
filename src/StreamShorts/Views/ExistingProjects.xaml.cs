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
using StreamShorts.MVVM.ViewModels;

namespace StreamShorts.Views;
/// <summary>
/// Interaction logic for ExistingProjects.xaml
/// </summary>
public partial class ExistingProjects : UserControl, IPage
{
  public ExistingProjects(ExistingProjectsViewModel viewModel) : this()
  {
    DataContext = viewModel;
  }
  public ExistingProjects()
  {
    InitializeComponent();
  }

  public string PageKey => "ExistingProjects";
}
