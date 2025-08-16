using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using StreamShorts.MVVM;
using StreamShorts.Projects;
using StreamShorts.Projects.Processing;
using StreamShorts.Views.Converters;

namespace StreamShorts.ViewModels;
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
  public ProjectDetailsViewModel(VideoQueueManager queueManager, VideoWorkItem videoWorkItem)
  {
    BackCommand = new DelegateCommand(OnBack);
    OpenVideoCommand = new DelegateCommand(OnOpenVideo);
    ProcessCommand = new DelegateCommand(async () => await OnProcess().ConfigureAwait(false));
    OpenFileCommand = new DelegateCommand<ProjectFileMediaElement>(OnOpenFile);
    PlayCommand = new DelegateCommand<MediaElement>(OnPlay);
    PauseCommand = new DelegateCommand<MediaElement>(OnPause);
    StopCommand = new DelegateCommand<MediaElement>(OnStop);
    _queueManager = queueManager;
    _videoWorkItem = videoWorkItem;
  }
  private Project? _project;
  private readonly VideoQueueManager _queueManager;
  private readonly VideoWorkItem _videoWorkItem;

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

  public static void OnBack()
  {
    NavigationEvent.Instance.PublishEvent("ExistingProjects", []);
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
      MessageBox.Show($"Unable to open video. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
  public async Task OnProcess()
  {
    Project!.IsProcessing = true;
    _videoWorkItem.Project = Project;
    await _queueManager.Enqueue(_videoWorkItem).ConfigureAwait(false);

  }
  public void OnOpenFile(ProjectFileMediaElement pfme)
  {
    if (pfme == null)
    {
      return;
    }
    VideoSource = new Uri(System.IO.Path.Combine(pfme.ProjectFile.Directory, pfme.ProjectFile.Name));
    CurrentVideo = pfme.ProjectFile.Name;
    pfme.MediaElement.Play();
  }
  public void OnPlay(MediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    //mediaElement.Source = VideoSource;
    mediaElement.Play();
  }
  public void OnPause(MediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    mediaElement.Pause();
  }
  public void OnStop(MediaElement mediaElement)
  {
    if (mediaElement == null || VideoSource == null)
    {
      return;
    }
    mediaElement.Stop();
  }
}