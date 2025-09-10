using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.Library;
public class SettingsRepo
{
  private readonly Settings _settings;

  public SettingsRepo(Settings settings)
  {
    _settings = settings;
  }

  public void SaveSettings()
  {
    // Save settings to a file or database
    DirectoryInfo dir = new DirectoryInfo(WorkingDirectory.Current);
    if (!dir.Exists)
    {
      dir.Create();
      return;
    }
    FileInfo settingsFile = new FileInfo(Path.Combine(dir.FullName, "settings.json"));

    // Create default settings file if it doesn't exist
    File.WriteAllText(settingsFile.FullName, System.Text.Json.JsonSerializer.Serialize(_settings));

  }

  public Settings LoadSettings()
  {
    DirectoryInfo dir = new DirectoryInfo(WorkingDirectory.Current);
    if (!dir.Exists)
    {
      dir.Create();
      return _settings;
    }
    FileInfo settingsFile = new FileInfo(Path.Combine(dir.FullName, "settings.json"));
    if (!settingsFile.Exists)
    {
      // Create default settings file if it doesn't exist
      File.WriteAllText(settingsFile.FullName, System.Text.Json.JsonSerializer.Serialize(_settings));
      return _settings;
    }
    // Load settings from the file
    string json = File.ReadAllText(settingsFile.FullName);
    Settings currsettings = System.Text.Json.JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
    _settings.BatchSize = currsettings.BatchSize;
    _settings.ChatGptApiKey = currsettings.ChatGptApiKey;
    _settings.ChatGptModelID = currsettings.ChatGptModelID;
    _settings.AzureOpenAIApiKey = currsettings.AzureOpenAIApiKey;
    _settings.AzureOpenAIEndPoint = currsettings.AzureOpenAIEndPoint;
    _settings.AzureOpenAIDeploymentName = currsettings.AzureOpenAIDeploymentName;
    _settings.AzureOpenAIModelID = currsettings.AzureOpenAIModelID;
    _settings.MaxClipLength = currsettings.MaxClipLength;
    _settings.GeminiEndpoint = currsettings.GeminiEndpoint;
    _settings.GeminiModelId = currsettings.GeminiModelId;
    _settings.IncludeFunnyClips = currsettings.IncludeFunnyClips;
    _settings.IncludeInformativeClips = currsettings.IncludeInformativeClips;
    _settings.IncludeInsightfulClips = currsettings.IncludeInsightfulClips;
    _settings.LLMProvider = currsettings.LLMProvider;
    _settings.LLMRetries = currsettings.LLMRetries;
    _settings.LLMTimeoutMinutes = currsettings.LLMTimeoutMinutes;
    _settings.MaxClipLength = currsettings.MaxClipLength;
    _settings.OllamaModelId = currsettings.OllamaModelId;

    // Load settings from a file or database
    return _settings;
  }
}
