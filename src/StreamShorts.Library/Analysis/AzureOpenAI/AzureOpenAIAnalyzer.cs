using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

using StreamShorts.Library.Analysis.Prompts;
using StreamShorts.Library.Transcription;

namespace StreamShorts.Library.Analysis.AzureOpenAI;

#pragma warning disable CA1816 // Implement IDisposable Correctly
#pragma warning disable CA1063 // Implement IDisposable Correctly
public class AzureOpenAIAnalyzer : ITranscriptAnalyzer, IDisposable
{
  //private readonly DefaultAnalysisPrompt _prompt = new DefaultAnalysisPrompt();
  private readonly Kernel _kernel;
  private readonly IAnalysisPrompt _analysisPrompt;
  private readonly Settings _settings;

  public AzureOpenAIAnalyzer(IAnalysisPrompt analysisPrompt, Settings settings)
  {

    _analysisPrompt = analysisPrompt;
    _settings = settings;

    //var modelId = "deepseek-r1:8b"; // nezahatkorkmaz/deepseek-v3:latest"; // System.Configuration.ConfigurationManager.AppSettings["OllamaModelId"]!;
    //var modelId = "nezahatkorkmaz/deepseek-v3:latest"; // System.Configuration.ConfigurationManager.AppSettings["OllamaModelId"]!;
    string modelId = string.IsNullOrWhiteSpace(_settings.ChatGptModelID) == false ? _settings.ChatGptModelID : "phi4:latest"; // System.Configuration.ConfigurationManager.AppSettings["OllamaModelId"]!;

    //Chat
    _kernel = Kernel.CreateBuilder()
      .AddAzureOpenAIChatCompletion(_settings.AzureOpenAIDeploymentName!, _settings.AzureOpenAIEndPoint!, _settings.AzureOpenAIApiKey!, modelId: _settings.AzureOpenAIModelID)
      .Build();
  }
  public async Task<TranscriptAnalysis> AnalyzeAsync(IEnumerable<TranscriptionSegment> segments)
  {


    var allSegments = segments.ToList();
    List<ShortClip> allClips = [];
    IChatCompletionService chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

    for (int i = 0; i < allSegments.Count; i += _settings.BatchSize)
    {
      var batch = allSegments.Skip(i).Take(_settings.BatchSize).ToList();


#pragma warning disable CA1031 // Do not catch general exception types
      int retries = 0;
      ChatHistory _chatHistory = [];
      _chatHistory.AddUserMessage(_analysisPrompt.GetPromptWrap(batch));
      string? json = null;
      while (retries < _settings.LLMRetries)
        try
        {


          OllamaPromptExecutionSettings settings = new();
          settings.ToChatOptions(_kernel)!.ResponseFormat = ChatResponseFormat.Json;
          var response = await chatCompletion.GetChatMessageContentAsync(_chatHistory, settings).ConfigureAwait(false);


          // Regex to match content between ```json and ```
          var match = Regex.Match(response.Content!, @"```json\s*(.*?)\s*```", RegexOptions.Singleline);
          if (match.Success)
          {
            json = match.Groups[1].Value;
          }
          else
          {
            json = response.Content!;
          }
          var clips = JsonSerializer.Deserialize<List<ShortClip>>(json ?? string.Empty);
          allClips.AddRange(clips ?? []);
          break;// Exit the retry loop if successful
        }
        catch (Exception ex)
        {
          Console.Write($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {json}");
          await Console.Error.WriteLineAsync(ex.Message).ConfigureAwait(false);
        }
#pragma warning restore CA1031 // Do not catch general exception types

    }
    return new TranscriptAnalysis(allClips ?? []);

  }
  public void Dispose()
  {
  }
}

#pragma warning restore CA1816 // Implement IDisposable Correctly
#pragma warning restore CA1063 // Implement IDisposable Correctly