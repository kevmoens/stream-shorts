using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;
using StreamShorts.MVVM.Projects.Processing;

using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.MVVM.ViewModels;
public class ProjectDetailsViewModel : INotifyPropertyChanged, IPageNavigationAware
{
  public event PropertyChangedEventHandler? PropertyChanged;
  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public ICommand BackCommand { get; set; }
  public ICommand OpenVideoCommand { get; set; }
  public ICommand ProcessCommand { get; set; }
  public ICommand OpenFileCommand { get; set; }
  public ICommand PlayCommand { get; set; }
  public ICommand PauseCommand { get; set; }
  public ICommand StopCommand { get; set; }
  public ProjectDetailsViewModel(VideoQueueManager queueManager, VideoWorkItem videoWorkItem, IMessageBox messageBox, INavigationEvent navigationEvent)
  {
    BackCommand = new DelegateCommand(async() => await OnBack().ConfigureAwait(false));
    OpenVideoCommand = new DelegateCommand(OnOpenVideo);
    ProcessCommand = new DelegateCommand(async () => await OnProcess().ConfigureAwait(false));
    OpenFileCommand = new DelegateCommand<IProjectFileMediaElement>(OnOpenFile);
    PlayCommand = new DelegateCommand<IMediaElement>(OnPlay);
    PauseCommand = new DelegateCommand<IMediaElement>(OnPause);
    StopCommand = new DelegateCommand<IMediaElement>(OnStop);
    _queueManager = queueManager;
    _videoWorkItem = videoWorkItem;
    _messageBox = messageBox;
    _navigationEvent = navigationEvent;
  }
  private Project? _project;
  private readonly VideoQueueManager _queueManager;
  private readonly VideoWorkItem _videoWorkItem;
  private readonly INavigationEvent _navigationEvent;
  private IMessageBox _messageBox;
  public IMessageBox MessageBox
  {
    get { return _messageBox; }
    set { _messageBox = value; OnPropertyChanged(); }
  }
  public Project? Project
  {
    get => _project;
    set
    {
      if (_project != value)
      {
        _project = value;
        OnPropertyChanged();
      }
    }
  }
  private Uri? _videoSource;

  public Uri? VideoSource
  {
    get { return _videoSource; }
    set { _videoSource = value; OnPropertyChanged(); }
  }
  private string? _currentVideo;

  public string? CurrentVideo
  {
    get { return _currentVideo; }
    set { _currentVideo = value; OnPropertyChanged(); }
  }

  public void OnNavigatedTo(Dictionary<string, object> parameters)
  {

    if (parameters != null
      && parameters.TryGetValue("Project", out object? projectObj)
      && projectObj is Project project)
    {
      Project = project;
    }


    if (parameters != null
    && parameters.TryGetValue("Status", out object? status)
    && status is string statusString
    && string.Equals(statusString, "New", StringComparison.OrdinalIgnoreCase))
    {
      //This is a new project, so we need to make it clear we need to let the user know it hasn't processed yet.
    }
  }

  public async Task OnBack()
  {
    await _navigationEvent.PublishEvent("ExistingProjects", []).ConfigureAwait(false);
  }
  public void OnOpenVideo()
  {
    if (Project?.VideoUri == null)
    {
      // Handle the case where the video URI is not set
      return;
    }
#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      var psi = new ProcessStartInfo
      {
        FileName = Project.VideoUri.AbsoluteUri,
        UseShellExecute = true
      };
      Process.Start(psi);
    }
    catch (Exception ex)
    {
      _messageBox.Show($"Unable to open video. {ex.Message}", "Error", MessageButtons.OK, MessageImage.Error);
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
  public async Task OnProcess()
  {
    Project!.IsProcessing = true;
    _videoWorkItem.Project = Project;
    await _queueManager.Enqueue(_videoWorkItem).ConfigureAwait(false);

  }
  public void OnOpenFile(IProjectFileMediaElement pfme)
  {
    if (pfme == null)
    {
      return;
    }
    VideoSource = new Uri(System.IO.Path.Combine(pfme.ProjectFile.Directory, pfme.ProjectFile.Name));
    CurrentVideo = pfme.ProjectFile.Name;
    pfme.MediaElement.Play();
  }
  public void OnPlay(IMediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    //mediaElement.Source = VideoSource;
    mediaElement.Play();
  }
  public void OnPause(IMediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    mediaElement.Pause();
  }
  public void OnStop(IMediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    mediaElement.Stop();
  }
}