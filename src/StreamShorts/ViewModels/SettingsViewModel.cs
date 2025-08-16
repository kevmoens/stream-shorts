using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using StreamShorts.Install;
using StreamShorts.Library;
using StreamShorts.Library.Analysis;
using StreamShorts.MVVM;
using StreamShorts.Projects;

namespace StreamShorts.ViewModels;
public class SettingsViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  private Settings _settings;
  public Settings Settings
  {
    get => _settings;
    set { _settings = value; OnPropertyChanged(); }
  }
  public ICommand LoadedCommand { get; set; }
  public ICommand SaveCommand { get; set; }
  public ICommand CancelCommand { get; set; }
  public ICommand LLMProviderChangedCommand { get; set; }
  public ICommand DownloadPhi4ModelCommand { get; set; }
  private readonly SettingsRepo _settingsRepo;

  public SettingsViewModel(Settings settings, SettingsRepo settingsRepo)
  {
    _settings = settings;
    _settingsRepo = settingsRepo;
    LoadedCommand = new DelegateCommand(OnLoaded);
    SaveCommand = new DelegateCommand(OnSave, CanSave);
    CancelCommand = new DelegateCommand(OnCancel);
    LLMProviderChangedCommand = new DelegateCommand(async() => await OnLLMProviderChanged().ConfigureAwait(false));
    DownloadPhi4ModelCommand = new DelegateCommand(OnDownloadPhi4Model);


  }
  private async void OnLoaded()
  {
    // Initialize properties from settings
    LLMProvider = _settings.LLMProvider;
    OllamaModelId = _settings.OllamaModelId;
    ChatGptApiKey = _settings.ChatGptApiKey;
    BatchSize = _settings.BatchSize;
    GeminiApiKey = _settings.GeminiApiKey;
    GeminiModelId = _settings.GeminiModelId;
    GeminiEndpoint = _settings.GeminiEndpoint;
    LLMRetries = _settings.LLMRetries;
    LLMTimeoutMinutes = _settings.LLMTimeoutMinutes;
    IncludeInformativeClips = _settings.IncludeInformativeClips;
    IncludeFunnyClips = _settings.IncludeFunnyClips;
    IncludeInsightfulClips = _settings.IncludeInsightfulClips;
    MaxClipLength = _settings.MaxClipLength;

    await OnLLMProviderChanged().ConfigureAwait(false);
  }
  private void OnSave()
  {
    _settings.LLMProvider = LLMProvider;
    _settings.OllamaModelId = OllamaModelId;
    _settings.ChatGptApiKey = ChatGptApiKey;
    _settings.BatchSize = BatchSize;
    _settings.GeminiApiKey = GeminiApiKey;
    _settings.GeminiModelId = GeminiModelId;
    _settings.GeminiEndpoint = GeminiEndpoint;
    _settings.LLMRetries = LLMRetries;
    _settings.LLMTimeoutMinutes = LLMTimeoutMinutes;
    _settings.IncludeInformativeClips = IncludeInformativeClips;
    _settings.IncludeFunnyClips = IncludeFunnyClips;
    _settings.IncludeInsightfulClips = IncludeInsightfulClips;
    _settings.MaxClipLength = MaxClipLength;
    _settingsRepo.SaveSettings();
    NavigationEvent.Instance.PublishEvent("ExistingProjects", []);
  }

  private void OnCancel()
  {
    NavigationEvent.Instance.PublishEvent("ExistingProjects", []);
  }
  private async Task OnLLMProviderChanged()
  {
    if (LLMProvider != LLMProvider.Ollama)
    {
      return;
    }
    if (OllamaModels.Count > 0)
    {
      return;
    }
    List<string>? models = await OllamaVerification.GetModels().ConfigureAwait(false);
    foreach (var model in models)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        OllamaModels.Add(model);
      });
    }
#pragma warning disable CA1507 // Use nameof to express symbol names
      OnPropertyChanged("LLMProvider"); //Fire again because we want the converter to run
#pragma warning restore CA1507 // Use nameof to express symbol names
  }
  public void OnDownloadPhi4Model()
  {
    var result = MessageBox.Show("Phi4:latest is a 9GB model.  This will take time to download.  Do you want to continue?", "Download Model", MessageBoxButton.YesNo);
    if (result != MessageBoxResult.Yes)
    { 
      return; 
    }
#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
        FileName = "ollama",
        Arguments = "run phi4:latest",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };
      using (Process process = new Process())
      {

        IsDownloadingPhi4 = true;
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        MessageBox.Show("Phi4:Latest is now downloaded, restarting app.");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
          FileName = Environment.ProcessPath!,
          UseShellExecute = true
        });
        Environment.Exit(0);
      }
    }
    finally 
    {
      IsDownloadingPhi4 = false;
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  // Properties for binding
#pragma warning disable CA1822 // Mark members as static
  public ObservableCollection<LLMProvider> LLMProviders
#pragma warning restore CA1822 // Mark members as static
  {
    get
    {
      return new ObservableCollection<LLMProvider>([LLMProvider.ChatGpt,
        LLMProvider.Ollama,
        LLMProvider.Gemini
      ]);
    }
  }
  private LLMProvider _llmProvider;
  public LLMProvider LLMProvider
  {
    get => _llmProvider;
    set { _llmProvider = value; OnPropertyChanged(); }
  }
  private ObservableCollection<string> _ollamaModels = [];

#pragma warning disable CA2227 // Collection properties should be read only
  public ObservableCollection<string> OllamaModels
#pragma warning restore CA2227 // Collection properties should be read only
  {
    get { return _ollamaModels; }
    set { _ollamaModels = value; OnPropertyChanged(); }
  }

  private string? _ollamaModelId;
  public string? OllamaModelId
  {
    get => _ollamaModelId;
    set { _ollamaModelId = value; OnPropertyChanged(); }
  }
  private string? _chatGptApiKey;
  public string? ChatGptApiKey
  {
    get => _chatGptApiKey;
    set { _chatGptApiKey = value; OnPropertyChanged(); }
  }
  private int _batchSize;
  public int BatchSize
  {
    get => _batchSize;
    set { _batchSize = value; OnPropertyChanged(); }
  }
  private string? _geminiApiKey;
  public string? GeminiApiKey
  {
    get => _geminiApiKey;
    set { _geminiApiKey = value; OnPropertyChanged(); }
  }
  private string? _geminiModelId;
  public string? GeminiModelId
  {
    get => _geminiModelId;
    set { _geminiModelId = value; OnPropertyChanged(); }
  }
  private string? _geminiEndpoint;
  public string? GeminiEndpoint
  {
    get => _geminiEndpoint;
    set { _geminiEndpoint = value; OnPropertyChanged(); }
  }
  private int _llmRetries;
  public int LLMRetries
  {
    get => _llmRetries;
    set { _llmRetries = value; OnPropertyChanged(); }
  }
  private int _llmTimeoutMinutes;
  public int LLMTimeoutMinutes
  {
    get => _llmTimeoutMinutes;
    set { _llmTimeoutMinutes = value; OnPropertyChanged(); }
  }
  private bool _includeInformativeClips;
  public bool IncludeInformativeClips
  {
    get => _includeInformativeClips;
    set { _includeInformativeClips = value; OnPropertyChanged(); }
  }
  private bool _includeFunnyClips;
  public bool IncludeFunnyClips
  {
    get => _includeFunnyClips;
    set { _includeFunnyClips = value; OnPropertyChanged(); }
  }
  private bool _includeInsightfulClips;
  public bool IncludeInsightfulClips
  {
    get => _includeInsightfulClips;
    set { _includeInsightfulClips = value; OnPropertyChanged(); }
  }
  private TimeSpan? _maxClipLength;
  public TimeSpan? MaxClipLength
  {
    get => _maxClipLength;
    set { _maxClipLength = value; OnPropertyChanged(); }
  }

  private bool _isDownloadingPhi4;

  public bool IsDownloadingPhi4
  {
    get { return _isDownloadingPhi4; }
    set { _isDownloadingPhi4 = value; OnPropertyChanged(); }
  }

#pragma warning disable CA1822 // Mark members as static
  private bool CanSave()
#pragma warning restore CA1822 // Mark members as static
  {
    // This assumes you only have one main window with the settings controls
    foreach (Window window in System.Windows.Application.Current.Windows)
    {
      if (HasValidationError(window))
        return false;
    }
    return true;
  }

  private static bool HasValidationError(DependencyObject obj)
  {
    if (System.Windows.Controls.Validation.GetHasError(obj))
      return true;

    for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
    {
      var child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
      if (HasValidationError(child))
        return true;
    }
    return false;
  }
}
