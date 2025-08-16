using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using StreamShorts.MVVM;

namespace StreamShorts.ViewModels;
public class InstallOllamaViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public ICommand OpenHttpLinkCommand { get; }
  public ICommand RestartAppCommand { get; }
  public InstallOllamaViewModel()
  {

    OpenHttpLinkCommand = new DelegateCommand(OpenOllamaLink);
    RestartAppCommand = new DelegateCommand(RestartApplication);
  }

  private void OpenOllamaLink()
  {
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
      FileName = "https://www.ollama.com/download",
      UseShellExecute = true
    });
  }

  private void RestartApplication()
  {
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
      FileName = Environment.ProcessPath!,
      UseShellExecute = true
    });
    Environment.Exit(0);
  }
}
