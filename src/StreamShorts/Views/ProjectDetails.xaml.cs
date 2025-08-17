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
/// Interaction logic for ProjectDetails.xaml
/// </summary>
public partial class ProjectDetails : UserControl, IPage
{
  public ProjectDetails(ProjectDetailsViewModel viewModel) : this()
  {
    DataContext = viewModel;
  }
  public ProjectDetails()
  {
    InitializeComponent();
  }

  public string PageKey => "ProjectDetails";
}
