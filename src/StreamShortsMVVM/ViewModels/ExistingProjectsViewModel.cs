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
  private readonly IMessageBox _messageBox;

  public ProjectFolderRepo ProjectRepo
  {
    get => _projectRepo;
  }
  public ICommand LoadedCommand { get; set; }
  public ICommand SettingsCommand { get; set; }
  public ICommand AddProjectCommand { get; set; }
  public ICommand OpenProjectCommand { get; set; }
  public ICommand DeleteProjectCommand { get; set; }
  public ExistingProjectsViewModel(ProjectFolderRepo projectRepo, Settings settings, IMessageBox messageBox)
  {
    LoadedCommand = new DelegateCommand(OnLoaded);
    SettingsCommand = new DelegateCommand(OnSettings);
    AddProjectCommand = new DelegateCommand(OnAddProject);
    OpenProjectCommand = new DelegateCommand<Project>(OnOpenProject);
    DeleteProjectCommand = new DelegateCommand<Project>(async (proj) => await OnDeleteProject(proj).ConfigureAwait(false));
    _projectRepo = projectRepo;
    _settings = settings;
    _messageBox = messageBox;
  }

  private async void OnLoaded()
  {
    await _projectRepo.LoadAll().ConfigureAwait(false);
  }

  private void OnSettings()
  {
    NavigationEvent.Instance.PublishEvent("Settings", []).ConfigureAwait(false);
  }

  private void OnAddProject()
  {
    if (InvalidSettings())
    {
      NavigationEvent.Instance.PublishEvent("Settings", []).ConfigureAwait(false);
      return;
    }
    NavigationEvent.Instance.PublishEvent("AddProject", []).ConfigureAwait(false);
  }
  private async void OnOpenProject(Project project)
  {
    if (InvalidSettings())
    {
      await NavigationEvent.Instance.PublishEvent("Settings", []).ConfigureAwait(false);
      return;
    }
    if (project == null)
    {
      _messageBox.Show("Please select a project to open.", "No Project Selected", MessageButtons.OK, MessageImage.Warning);
      return;
    }
    await NavigationEvent.Instance.PublishEvent("ProjectDetails", new Dictionary<string, object> { { "Status", "Exists" }, { "Project", project } }).ConfigureAwait(false);
  }
  private async Task OnDeleteProject(Project project)
  {
    if (project == null)
    {
      _messageBox.Show("Please select a project to delete.", "No Project Selected", MessageButtons.OK, MessageImage.Warning);
      return;
    }
    var result = _messageBox.Show($"Are you sure you want to delete the project '{project.ProjectName}'?", "Confirm Delete", MessageButtons.YesNo, MessageImage.Warning);
    if (result == MessageButtons.Yes)
    {
      await ProjectRepo.DeleteProject(project).ConfigureAwait(false);
    }
  }
  private bool InvalidSettings()
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
