using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using StreamShorts.Library.Analysis;
using StreamShorts.Library.Analysis.Gemini;
using StreamShorts.Library.Analysis.Ollama;
using StreamShorts.Library.Analysis.Prompts;
using StreamShorts.Library.Media.Audio;
using StreamShorts.Library.Transcription;

namespace StreamShorts.Library;
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddStreamShorts(this IServiceCollection services)
  {
    services.AddTransient<IAnalysisPrompt, DefaultAnalysisPrompt>();
    services.AddSingleton<IAudioExtractor, AudioExtractor>();
    services.AddSingleton<ITranscriber, WhisperTranscriber>();
    services.AddKeyedSingleton<ITranscriptAnalyzer, GeminiAnalyzer>(LLMProvider.Gemini.ToString());
    services.AddKeyedSingleton<ITranscriptAnalyzer, OllamaAnalyzer>(LLMProvider.Ollama.ToString());
    services.AddSingleton<Settings>();
    services.AddSingleton<SettingsRepo>();
    return services;
  }
}
