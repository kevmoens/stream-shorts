using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using StreamShorts.Install;
using StreamShorts.Library;
using StreamShorts.MVVM;
using StreamShorts.Projects;
using StreamShorts.Projects.Processing;

namespace StreamShorts.ViewModels
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private object? _content;

    public object? Content
    {
      get { return _content; }
      set { _content = value; OnPropertyChanged(); }
    }

    private string _Title = "Stream Shorts";

    public string Title
    {
      get { return _Title; }
      set { _Title = value; OnPropertyChanged(); }
    }

    public ICommand LoadedCommand { get; set; }
    public ICommand ClosingCommand { get; set; }

    private readonly IFactory<IPage> _pageFactory;
    private readonly SettingsRepo _settingsRepo;
    private VideoQueueManager _videoQueueManager;

    public VideoQueueManager VideoQueueManager
    {
      get { return _videoQueueManager; }
      set { _videoQueueManager = value; OnPropertyChanged(); }
    }


    public MainWindowViewModel([FromKeyedServices("ExistingProjects")] IPage existingProjectsPage,
                               IFactory<IPage> pageFactory,
                               SettingsRepo settingsRepo,
                               VideoQueueManager videoQueueManager
                               )
    {
      _pageFactory = pageFactory;
      _settingsRepo = settingsRepo;
      _videoQueueManager = videoQueueManager;
      Content = existingProjectsPage;
      NavigationEvent.Instance.SubscribeToEvent(OnNavigationEvent);
      LoadedCommand = new DelegateCommand(OnLoaded);
      ClosingCommand = new DelegateCommand(OnClosing);
    }

    private async void OnLoaded()
    {
      _settingsRepo.LoadSettings();
      if (await FFMpegVerification.IsFFMpegInstalled().ConfigureAwait(false) == false)
      {
        // Handle FFMpeg not installed
        NavigationEvent.Instance.PublishEvent("InstallFFMpeg", []);
      }
    }

    private void OnClosing()
    {
    }

    private void OnNavigationEvent(object? sender, NavigationEventArgs? e)
    {
      if (e == null) return;

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        var view = _pageFactory.Create(e.Page);
        Title = $"Stream Shorts {e.Page}";
        Content = view;
        if (view?.DataContext is IPageNavigationAware vm)
        {
          vm.OnNavigatedTo(e.Parms);
        }
      });
    }
  }
}
