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
using StreamShorts.Projects;

namespace StreamShorts.ViewModels;
public class ExistingProjectsViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private readonly ProjectFolderRepo _projectRepo;
  private readonly Settings _settings;

  public ProjectFolderRepo ProjectRepo
  {
    get => _projectRepo;
  }
  public ICommand LoadedCommand { get; set; }
  public ICommand SettingsCommand { get; set; }
  public ICommand AddProjectCommand { get; set; }
  public ICommand OpenProjectCommand { get; set; }
  public ICommand DeleteProjectCommand { get; set; }
  public ExistingProjectsViewModel(ProjectFolderRepo projectRepo, Settings settings)
  {
    LoadedCommand = new DelegateCommand(OnLoaded);
    SettingsCommand = new DelegateCommand(OnSettings);
    AddProjectCommand = new DelegateCommand(OnAddProject);
    OpenProjectCommand = new DelegateCommand<Project>(OnOpenProject);
    DeleteProjectCommand = new DelegateCommand<Project>(OnDeleteProject);
    _projectRepo = projectRepo;
    _settings = settings;
  }

  private async void OnLoaded()
  {
    await _projectRepo.LoadAll().ConfigureAwait(false);
  }

  private void OnSettings()
  {
    NavigationEvent.Instance.PublishEvent("Settings", []);
  }

  private void OnAddProject()
  {
    if (InvalidSettings())
    {
      NavigationEvent.Instance.PublishEvent("Settings", []);
      return;
    }
    NavigationEvent.Instance.PublishEvent("AddProject", []);
  }
  private void OnOpenProject(Project project)
  {
    if (InvalidSettings())
    {
      NavigationEvent.Instance.PublishEvent("Settings", []);
      return;
    }
    if (project == null)
    {
      MessageBox.Show("Please select a project to open.", "No Project Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    NavigationEvent.Instance.PublishEvent("ProjectDetails", new Dictionary<string, object> { { "Status", "Exists" }, { "Project", project } });
  }
  private void OnDeleteProject(Project project)
  {
    if (project == null)
    {
      MessageBox.Show("Please select a project to delete.", "No Project Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
      return;
    }
    var result = MessageBox.Show($"Are you sure you want to delete the project '{project.ProjectName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
    if (result == MessageBoxResult.Yes)
    {
      ProjectRepo.DeleteProject(project);
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
      MessageBox.Show("Gemini is not yet supported. Please select a different LLM provider.", "Unsupported LLM Provider", MessageBoxButton.OK, MessageBoxImage.Warning);
      return true;
    }
    if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey) && _settings.LLMProvider == LLMProvider.Gemini)
    {
      return true;
    }
    return false;
  }
}
