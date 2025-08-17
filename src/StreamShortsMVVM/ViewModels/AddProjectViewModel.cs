using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using StreamShorts.Library;
using StreamShorts.MVVM;
using StreamShorts.MVVM.Interfaces;
using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;
using StreamShorts.MVVM.YouTube;

namespace StreamShorts.MVVM.ViewModels;
public class AddProjectViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private readonly EventHandler _youTubeUrlChanged;
  public ICommand LoadedCommand { get; set; }
  public ICommand AddProjectCommand { get; set; }
  public ICommand CancelCommand { get; set; }
  public AddProjectViewModel(ProjectFolderRepo projectRepo, YouTubeDownload youTubeDownload, Project project, IMessageBox messageBox)
  {
    LoadedCommand = new DelegateCommand(OnLoaded);
    AddProjectCommand = new DelegateCommand(OnAddProject);
    CancelCommand = new DelegateCommand(OnCancel);
    _projectRepo = projectRepo;
    _youTubeDownload = youTubeDownload;
    _project = project;
    _messageBox = messageBox;
    _youTubeUrlChanged += async (sender, args) => await OnValidateYouTubeUrl().ConfigureAwait(false);
  }

  private string _youTubeUrl = string.Empty;

#pragma warning disable CA1056 // URI-like properties should not be strings
  public string YouTubeUrl
#pragma warning restore CA1056 // URI-like properties should not be strings
  {
    get { return _youTubeUrl; }
    set
    {
      _youTubeUrl = value;
      _youTubeUrlChanged?.Invoke(this, EventArgs.Empty);
      OnPropertyChanged();
    }
  }
  private string _projectName = string.Empty;
  private readonly ProjectFolderRepo _projectRepo;
  private readonly YouTubeDownload _youTubeDownload;
  private readonly Project _project;
  private readonly IMessageBox _messageBox;

  public string ProjectName
  {
    get { return _projectName; }
    set { _projectName = value; OnPropertyChanged(); }
  }

  private void OnLoaded()
  {
    YouTubeUrl = string.Empty;
    ProjectName = string.Empty;
  }
  private async Task OnValidateYouTubeUrl()
  {
#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      if (string.IsNullOrWhiteSpace(YouTubeUrl))
      {
        return;
      }
      _youTubeDownload.URL = new Uri(YouTubeUrl);
      string projectName = await _youTubeDownload.GetVideoName().ConfigureAwait(false);

      // Remove invalid filename characters
      var invalidChars = Path.GetInvalidFileNameChars();
      projectName = string.Concat(projectName.Where(c => !invalidChars.Contains(c)));

      ProjectName = projectName;
    }
    catch (UriFormatException)
    {
      _messageBox.Show("Invalid YouTube URL format.", "Stream Shorts", MessageButtons.OK, MessageImage.None);
      YouTubeUrl = string.Empty;
      ProjectName = string.Empty;
    }
    catch (Exception ex)
    {
      _messageBox.Show($"Error validating YouTube URL: {ex.Message}", "Stream Shorts", MessageButtons.OK, MessageImage.Error);
      YouTubeUrl = string.Empty;
      ProjectName = string.Empty;
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
  private async void OnAddProject()
  {

#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      ArgumentNullException.ThrowIfNullOrWhiteSpace(YouTubeUrl);
      ArgumentNullException.ThrowIfNullOrWhiteSpace(ProjectName);

      // Remove invalid filename characters
      var invalidChars = Path.GetInvalidFileNameChars();
      string projectName = string.Concat(ProjectName.Where(c => !invalidChars.Contains(c)));
      if (ProjectName != projectName)
      {
        if (_messageBox.Show("Project name contained invalid characters.  They have been removed.", "Invalid Characters", MessageButtons.OKCancel, MessageImage.None) == MessageButtons.Cancel )
        {
          return;
        }
      }
      ProjectName = projectName;
      _project.ProjectName = ProjectName;
      _project.VideoUri = new Uri(YouTubeUrl);

      _youTubeDownload.URL = new Uri(YouTubeUrl);

      _project.LocalVideoPath = ProjectName + ".mp4";
      DirectoryInfo dir = new(Path.Combine(WorkingDirectory.Current, ProjectName));
      if (dir.Exists)
      {
        _messageBox.Show("Project already exists.", "Stream Shorts", MessageButtons.OK, MessageImage.Error);
        return;
      }
      dir.Create();
      string previewPath = Path.Combine(dir.FullName, "preview.jpg");
      byte[] previewBytes = await _youTubeDownload.GetThumbnail().ConfigureAwait(false);
      await File.WriteAllBytesAsync(previewPath, previewBytes).ConfigureAwait(false);
      _project.Preview = "preview.jpg";
      string json = JsonSerializer.Serialize(_project);
      await File.WriteAllTextAsync(Path.Combine(dir.FullName, "details.json"), json).ConfigureAwait(false);
      await _projectRepo.AddProject(_project).ConfigureAwait(false);
      await NavigationEvent.Instance.PublishEvent("ProjectDetails", new Dictionary<string, object> { { "Status", "New" }, { "Project", _project } }).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      _messageBox.Show(ex.Message, "Stream Shorts", MessageButtons.OK, MessageImage.Error);
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
  private async void OnCancel()
  {
    await NavigationEvent.Instance.PublishEvent("ExistingProjects", []).ConfigureAwait(false);
  }
}
