using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using StreamShorts.Library.Analysis;

using Whisper.net.Logger;

namespace StreamShorts.Library;
public class Settings : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;
  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  // Private fields
  private LLMProvider _llmProvider = LLMProvider.ChatGpt;
  private string? _ollamaModelId;
  private string? _chatGptApiKey;
  private int _batchSize = 25;
  private string? _geminiApiKey;
  private string? _geminiModelId;
  private string? _geminiEndpoint;
  private int _llmRetries = 3;
  private int _llmTimeoutMinutes = 10;
  private bool _includeInformativeClips = true;
  private bool _includeFunnyClips = true;
  private bool _includeInsightfulClips = true;
  private TimeSpan? _maxClipLength = TimeSpan.FromMinutes(1);

  // Properties for binding
  public LLMProvider LLMProvider
  {
    get => _llmProvider;
    set
    {
      if (_llmProvider != value)
      {
        _llmProvider = value;
        OnPropertyChanged();
      }
    }
  }

  public string? OllamaModelId
  {
    get => _ollamaModelId;
    set
    {
      if (_ollamaModelId != value)
      {
        _ollamaModelId = value;
        OnPropertyChanged();
      }
    }
  }

  public string? ChatGptApiKey
  {
    get => _chatGptApiKey;
    set
    {
      if (_chatGptApiKey != value)
      {
        _chatGptApiKey = value;
        OnPropertyChanged();
      }
    }
  }

  public int BatchSize
  {
    get => _batchSize;
    set
    {
      if (_batchSize != value)
      {
        _batchSize = value;
        OnPropertyChanged();
      }
    }
  }

  public string? GeminiApiKey
  {
    get => _geminiApiKey;
    set
    {
      if (_geminiApiKey != value)
      {
        _geminiApiKey = value;
        OnPropertyChanged();
      }
    }
  }

  public string? GeminiModelId
  {
    get => _geminiModelId;
    set
    {
      if (_geminiModelId != value)
      {
        _geminiModelId = value;
        OnPropertyChanged();
      }
    }
  }

  public string? GeminiEndpoint
  {
    get => _geminiEndpoint;
    set
    {
      if (_geminiEndpoint != value)
      {
        _geminiEndpoint = value;
        OnPropertyChanged();
      }
    }
  }

  public int LLMRetries
  {
    get => _llmRetries;
    set
    {
      if (_llmRetries != value)
      {
        _llmRetries = value;
        OnPropertyChanged();
      }
    }
  }

  public int LLMTimeoutMinutes
  {
    get => _llmTimeoutMinutes;
    set
    {
      if (_llmTimeoutMinutes != value)
      {
        _llmTimeoutMinutes = value;
        OnPropertyChanged();
      }
    }
  }

  public bool IncludeInformativeClips
  {
    get => _includeInformativeClips;
    set
    {
      if (_includeInformativeClips != value)
      {
        _includeInformativeClips = value;
        OnPropertyChanged();
      }
    }
  }

  public bool IncludeFunnyClips
  {
    get => _includeFunnyClips;
    set
    {
      if (_includeFunnyClips != value)
      {
        _includeFunnyClips = value;
        OnPropertyChanged();
      }
    }
  }

  public bool IncludeInsightfulClips
  {
    get => _includeInsightfulClips;
    set
    {
      if (_includeInsightfulClips != value)
      {
        _includeInsightfulClips = value;
        OnPropertyChanged();
      }
    }
  }

  public TimeSpan? MaxClipLength
  {
    get => _maxClipLength;
    set
    {
      if (_maxClipLength != value)
      {
        _maxClipLength = value;
        OnPropertyChanged();
      }
    }
  }
}
