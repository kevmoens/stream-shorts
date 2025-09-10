using System.Globalization;
using System.Text;
using System.Xml;

using Microsoft.VisualBasic;

using StreamShorts.Library.Transcription;

using static System.Collections.Specialized.BitVector32;

namespace StreamShorts.Library.Analysis.Prompts;

/// <summary>
/// Default implementation of the analysis prompt for generating YouTube Shorts.
/// </summary>
/// <inheritdoc/>
public sealed class DefaultAnalysisPrompt : IAnalysisPrompt
{
  public DefaultAnalysisPrompt(Settings settings)
  {
    _settings = settings;
  }
 
  private static readonly CompositeFormat Prompt = CompositeFormat.Parse(@"
  I need your help to transform my YouTube live stream transcript into engaging YouTube Shorts. Act as my content editor and pinpoint **all potential candidate segments** that are perfect for short-form video. I'm looking for clips that are:
  
   --1--

  For each suggested short, please provide:

    - The **start time** of the initial segment and the **end time** of the final segment. The duration of each short should be no longer than 3 minutes, but **aim for durations between 15 seconds and 60 seconds**. However, the short **must be as long as necessary to capture the complete thought or idea**, even if it means exceeding the target range or extending slightly to capture all necessary dialogue.
    - A concise **title** that grabs attention.
    - A brief **description** highlighting the short's content and its appeal.
    - An **explanation** of why this particular segment is suitable for a YouTube Short, focusing on its potential for discoverability and engagement.

  Please format your response as a JSON array of objects with the following structure:

  ```json
  {{
    ""title"": ""string"",
    ""start_time"": ""TimeSpan"",
    ""end_time"": ""TimeSpan"",
    ""description"": ""string"",
    ""explanation"": ""string""
  }}
  ```

  Here is the transcript of my YouTube live stream below with the format of {{start_time}}  : {{text}}:

  {0}
  ");
  private readonly Settings _settings;

  public string GetPrompt(IEnumerable<TranscriptionSegment> transcript)
  {
    return UseSettings(string.Format(CultureInfo.InvariantCulture, Prompt, string.Join('\n', transcript.Select(script => $"{script.StartTime} - {script.EndTime} : {script.Text}"))));
  }
  public string GetPromptWrap(IEnumerable<TranscriptionSegment> transcript)
  {
    return UseSettings(string.Format(CultureInfo.InvariantCulture, Prompt, string.Join('\n', transcript.Select(script => $"{script.StartTime} : {script.Text}"))) 
      + '\n' + string.Format(CultureInfo.InvariantCulture, Prompt, ""));
  }
  private string UseSettings(string prompt)
  {
    StringBuilder sb = new StringBuilder(prompt);
    if (_settings.IncludeFunnyClips)
    {
      sb.AppendLine("- **Funny:** Moments that will make viewers laugh.");
    }
    if (_settings.IncludeInformativeClips)
    {
      sb.AppendLine("- **Informative:** Sections packed with valuable information or tips.");
    }
    if (_settings.IncludeInsightfulClips)
    {
      sb.AppendLine("- **Insightful:** Portions offering unique perspectives or 'aha!' moments.");
    }
    sb.AppendLine("- **High Engagement Potential:** Clips that contain laughing or high energy.");
#pragma warning disable CA1307 // Specify StringComparison for clarity
    return prompt.Replace("--1--", sb.ToString());
#pragma warning restore CA1307 // Specify StringComparison for clarity
  }
}