using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using StreamShorts.MVVM.Install;
using StreamShorts.Library;
using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;
using StreamShorts.MVVM.Projects.Processing;
using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.MVVM.ViewModels
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
    private readonly IUiDispatcher _uiDispatcher;
    private readonly INavigationEvent _navigationEvent;

    public VideoQueueManager VideoQueueManager
    {
      get { return _videoQueueManager; }
      set { _videoQueueManager = value; OnPropertyChanged(); }
    }


    public MainWindowViewModel([FromKeyedServices("ExistingProjects")] IPage existingProjectsPage,
                               IFactory<IPage> pageFactory,
                               SettingsRepo settingsRepo,
                               VideoQueueManager videoQueueManager,
                               IUiDispatcher uiDispatcher,
                               INavigationEvent navigationEvent
                               )
    {
      _pageFactory = pageFactory;
      _settingsRepo = settingsRepo;
      _videoQueueManager = videoQueueManager;
      _uiDispatcher = uiDispatcher;
      _navigationEvent = navigationEvent;
      Content = existingProjectsPage;
      _navigationEvent.SubscribeToEvent(OnNavigationEvent);
      LoadedCommand = new DelegateCommand(OnLoaded);
      ClosingCommand = new DelegateCommand(OnClosing);
    }

    private async void OnLoaded()
    {
      _settingsRepo.LoadSettings();
      if (await FFMpegVerification.IsFFMpegInstalled().ConfigureAwait(false) == false)
      {
        // Handle FFMpeg not installed
        await _navigationEvent.PublishEvent("InstallFFMpeg", []).ConfigureAwait(false);
      }
    }

    private void OnClosing()
    {
    }

    private async Task OnNavigationEvent(NavigationEventArgs? e)
    {
      if (e == null) return;

      await _uiDispatcher.InvokeAsync(() =>
      {

        var view = _pageFactory.Create(e.Page);
        Title = $"Stream Shorts {e.Page}";
        Content = view;
        if (view?.DataContext is IPageNavigationAware vm)
        {
          vm.OnNavigatedTo(e.Parms);
        }
        return Task.CompletedTask;
      }).ConfigureAwait(false);
    
    }
  }
}
