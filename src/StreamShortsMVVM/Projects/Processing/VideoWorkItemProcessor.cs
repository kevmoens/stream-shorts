using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FFMpegCore;

using StreamShorts.Library;
using StreamShorts.Library.Analysis;
using StreamShorts.Library.Transcription;
using StreamShorts.MVVM;
using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.YouTube;
using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.MVVM.Projects.Processing;
public class VideoWorkItemProcessor
{
  private readonly IFileSystem _fileSystem;
  private readonly ITranscriber _transcriber;
  private readonly IFactory<ITranscriptAnalyzer> _transcriptAnalyzerFactory;
  private readonly Settings _settings;
  private readonly YouTubeDownload _youTubeDownload;
  private readonly IUiDispatcher _uiDispatcher;
  public VideoWorkItemProcessor(
    IFileSystem fileSystem,
    ITranscriber transcriber,
    IFactory<ITranscriptAnalyzer> transcriptAnalyzerFactory,
    Settings settings,
    YouTubeDownload youTubeDownload,
    IUiDispatcher uiDispatcher
    )
  {
    _fileSystem = fileSystem;
    _transcriber = transcriber;
    _transcriptAnalyzerFactory = transcriptAnalyzerFactory;
    _settings = settings;
    _youTubeDownload = youTubeDownload;
    _uiDispatcher = uiDispatcher;
  }
  public async IAsyncEnumerable<string> ProcessAsync(VideoWorkItem workItem)
  {
    ArgumentNullException.ThrowIfNull(workItem);
    ArgumentNullException.ThrowIfNull(workItem.Project);
    Stopwatch stopwatch = Stopwatch.StartNew();

    _youTubeDownload.URL = workItem.Project.VideoUri;
    _youTubeDownload.FolderPath = Path.Combine(WorkingDirectory.Current, workItem.Project.ProjectName!);
    yield return $"Downloading video: {workItem.Project.VideoUri}";
    string videoPath = Path.Combine(_youTubeDownload.FolderPath, await _youTubeDownload.GetVideoName().ConfigureAwait(false)) + ".mp4";
    if (!File.Exists(videoPath))
    {
#pragma warning disable CA1031 // Do not catch general exception types
      string? errorMessage = null;
      try
      {
        videoPath = await _youTubeDownload.GetVideo().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        errorMessage = $"Error downloading video: {ex.Message}";
      }
      if (errorMessage != null)
      {
        workItem.Project.IsProcessing = false;
        yield return errorMessage;
        yield break;
      }
#pragma warning restore CA1031 // Do not catch general exception types
    }
    workItem.Project.LocalVideoPath = videoPath;

    yield return $"Processing stream: {workItem.Project.LocalVideoPath}";
    var videoStream = _fileSystem.File.OpenRead(workItem.Project.LocalVideoPath);

    Stream? audioStream = null;

    var outputPath = _fileSystem.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".mp3");
    FFMpeg.ExtractAudio(workItem.Project.LocalVideoPath, outputPath);
    audioStream = _fileSystem.File.OpenRead(outputPath);

    if (audioStream is null)
    {
      yield return "Failed to extract audio from the stream.";
      yield break;
    }

    yield return $"Audio extracted  successfully!";

    List<TranscriptionSegment> transcriptionSegments = [];

    yield return "Transcribing audio...";
    await foreach (var segment in _transcriber.TranscribeAsync(audioStream).ConfigureAwait(false))
    {
      transcriptionSegments.Add(segment);
      var segmentTimeText = $"[{segment.StartTime:hh\\:mm\\:ss} - {segment.EndTime:hh\\:mm\\:ss}]";
      yield return $"Transcribed segment: {segmentTimeText}";
    }

    yield return $"Transcription completed  successfully!";

    TranscriptAnalysis? analysis = null;

    yield return "Analyzing transcript...";
    var transcriptAnalyzer = _transcriptAnalyzerFactory.Create(_settings.LLMProvider.ToString());
    analysis = await transcriptAnalyzer!.AnalyzeAsync(transcriptionSegments).ConfigureAwait(false);

    foreach (var clip in analysis!.ShortClips)
    {
      if (clip.EndTime < clip.StartTime)
      {
        continue;
      }
      var clipOutputPath = _fileSystem.Path.Combine(
        _fileSystem.Path.GetDirectoryName(workItem.Project.LocalVideoPath) ?? string.Empty,
        $"{_fileSystem.Path.GetFileNameWithoutExtension(workItem.Project.LocalVideoPath)}_clip_{clip.StartTime:hhmmss}_{clip.EndTime:hhmmss}.mp4"
      );

      // Use FFMpegCore to extract the clip from the original video
      await FFMpeg.SubVideoAsync(workItem.Project.LocalVideoPath, clipOutputPath, clip.StartTime, clip.EndTime).ConfigureAwait(false);
      FileInfo subFile = new(clipOutputPath);
      await _uiDispatcher.InvokeAsync(() =>
      {
        workItem.Project.Files.Add(new ProjectFile(workItem.Project, subFile.Name) { Description = clip.Description, Duration = clip.EndTime - clip.StartTime });
        return Task.CompletedTask;
      }).ConfigureAwait(false);
      yield return $"Clip created: {clipOutputPath}";
    }

    //Clean up temporary audio file
    if (File.Exists(outputPath))
    {
      audioStream.Close();
      _fileSystem.File.Delete(outputPath);
      yield return $"Temporary audio file deleted: {outputPath}";
    }

    string json = JsonSerializer.Serialize(workItem.Project);
    await File.WriteAllTextAsync(Path.Combine(WorkingDirectory.Current, workItem.Project.ProjectName!, "details.json"), json).ConfigureAwait(false);

    stopwatch.Stop();
    yield return $"Process completed in {stopwatch.Elapsed.TotalMinutes} minutes!";
  }
}
