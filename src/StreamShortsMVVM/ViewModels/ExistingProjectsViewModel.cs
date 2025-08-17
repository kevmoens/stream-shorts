using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using StreamShorts.Library;
using StreamShorts.Library.Analysis;
using StreamShorts.MVVM;
using StreamShorts.MVVM.Interfaces;
using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;

namespace StreamShorts.MVVM.ViewModels;
public class ExistingProjectsViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private readonly ProjectFolderRepo _projectRepo;
  private readonly Settings _settings;
  private readonly INavigationEvent _navigationEvent;
  private IMessageBox _messageBox;
    public IMessageBox MessageBox
  {
    get { return _messageBox; }
    set { _messageBox = value; OnPropertyChanged(); }
  }

  public ProjectFolderRepo ProjectRepo
  {
    get => _projectRepo;
  }
  public ICommand LoadedCommand { get; set; }
  public ICommand SettingsCommand { get; set; }
  public ICommand AddProjectCommand { get; set; }
  public ICommand OpenProjectCommand { get; set; }
  public ICommand DeleteProjectCommand { get; set; }
  public ExistingProjectsViewModel(ProjectFolderRepo projectRepo, Settings settings, IMessageBox messageBox, INavigationEvent navigationEvent)
  {
    LoadedCommand = new DelegateCommand(async () => await OnLoaded().ConfigureAwait(false));
    SettingsCommand = new DelegateCommand(async () => await OnSettings().ConfigureAwait(false));
    AddProjectCommand = new DelegateCommand(async () => await OnAddProject().ConfigureAwait(false));
    OpenProjectCommand = new DelegateCommand<Project>(async (proj) => await OnOpenProject(proj).ConfigureAwait(false));
    DeleteProjectCommand = new DelegateCommand<Project>(async (proj) => await OnDeleteProject(proj).ConfigureAwait(false));
    _projectRepo = projectRepo;
    _settings = settings;
    _messageBox = messageBox;
    _navigationEvent = navigationEvent;
  }

  public async Task OnLoaded()
  {
    await _projectRepo.LoadAll().ConfigureAwait(false);
  }

  public async Task OnSettings()
  {
    await _navigationEvent.PublishEvent("Settings", []).ConfigureAwait(false);
  }

  public async Task OnAddProject()
  {
    if (InvalidSettings())
    {
      await _navigationEvent.PublishEvent("Settings", []).ConfigureAwait(false);
      return;
    }
    await _navigationEvent.PublishEvent("AddProject", []).ConfigureAwait(false);
  }
  public async Task OnOpenProject(Project project)
  {
    if (InvalidSettings())
    {
      await _navigationEvent.PublishEvent("Settings", []).ConfigureAwait(false);
      return;
    }
    if (project == null)
    {
      await _messageBox.Show("Please select a project to open.", "No Project Selected", MessageButtons.OK, MessageImage.Warning).ConfigureAwait(false);
      return;
    }
    await _navigationEvent.PublishEvent("ProjectDetails", new Dictionary<string, object> { { "Status", "Exists" }, { "Project", project } }).ConfigureAwait(false);
  }
  public async Task OnDeleteProject(Project project)
  {
    if (project == null)
    {
      await _messageBox.Show("Please select a project to delete.", "No Project Selected", MessageButtons.OK, MessageImage.Warning).ConfigureAwait(false);
      return;
    }
    var result = await _messageBox.Show($"Are you sure you want to delete the project '{project.ProjectName}'?", "Confirm Delete", MessageButtons.YesNo, MessageImage.Warning).ConfigureAwait(false);
    if (result == MessageButtons.Yes)
    {
      await ProjectRepo.DeleteProject(project).ConfigureAwait(false);
    }
  }
  public bool InvalidSettings()
  {
    if (string.IsNullOrWhiteSpace(_settings.ChatGptApiKey) && _settings.LLMProvider == LLMProvider.ChatGpt)
    {
      return true;
    }
    if (string.IsNullOrWhiteSpace(_settings.OllamaModelId) && _settings.LLMProvider == LLMProvider.Ollama)
    {
      return true;
    }
    if (_settings.LLMProvider == LLMProvider.Gemini)
    {
      _messageBox.Show("Gemini is not yet supported. Please select a different LLM provider.", "Unsupported LLM Provider", MessageButtons.OK, MessageImage.Warning);
      return true;
    }
    if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey) && _settings.LLMProvider == LLMProvider.Gemini)
    {
      return true;
    }
    return false;
  }
}
