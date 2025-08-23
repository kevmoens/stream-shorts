using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;

using StreamShorts.Library.Media;

using Whisper.net;
using Whisper.net.Ggml;

namespace StreamShorts.Library.Transcription;

/// <summary>
/// Represents a transcriber that uses Whisper for audio transcription.
/// </summary>
/// <inheritdoc/>
public sealed class WhisperTranscriber : ITranscriber, IDisposable
{
  private readonly IAudioService _audioService = new NAudioService();
  private readonly Settings _settings = new Settings();
  private WhisperProcessor? _whisperProcessor;

  /// <summary>
  /// Initializes a new instance of the <see cref="WhisperTranscriber"/> class.
  /// </summary>
  public WhisperTranscriber()
  {

  }

  /// <summary>
  /// Initializes a new instance of the <see cref="WhisperTranscriber"/> class with a specified audio service.
  /// </summary>
  /// <param name="audioService">The audio service to use for audio processing.</param>
  /// <exception cref="ArgumentNullException">Thrown when the audio service is null.</exception
  internal WhisperTranscriber(IAudioService audioService, Settings settings)
  {
    _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService), $"{nameof(audioService)} cannot be null");
    _settings = settings;
  }

  public async IAsyncEnumerable<TranscriptionSegment> TranscribeAsync(Stream audio, [EnumeratorCancellation] CancellationToken cancellationToken)
  {

    var segmentDuration = _settings.MaxClipLength ?? TimeSpan.FromSeconds(30);
    var wavStream = _audioService.ConvertMp3ToWav16(audio);
    var tempWaveFile = System.IO.Path.Combine(System.Environment.CurrentDirectory, "temp.wav");
    System.Console.WriteLine(tempWaveFile);
#pragma warning disable CA1031 // Do not catch general exception types

    try
    {
      using (var fileStream = new System.IO.FileStream(tempWaveFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
      {
        await wavStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error writing WAV file: {ex.Message}");
    }
#pragma warning restore CA1031 // Do not catch general exception types

    var numberOfSegments = _audioService.GetNumberOfWavSegments(wavStream, segmentDuration);

    foreach (var segmentNumber in Enumerable.Range(0, numberOfSegments))
    {
      var segmentStream = _audioService.GetWavSegment(wavStream, segmentNumber, segmentDuration);
      var durationOffset = TimeSpan.FromMilliseconds(segmentNumber * segmentDuration.TotalMilliseconds);

      StringBuilder sb = new();
      TimeSpan? start = null;
      TimeSpan? end = null;
      await foreach (var result in ProcessSegmentAsync(segmentStream, cancellationToken).ConfigureAwait(false))
      {
        if (start is null)
        {
          start = result.Start;
        }
        end = result.End;
        sb.Append(result.Text);
        //yield return new TranscriptionSegment(
        //  result.Start + durationOffset,
        //  result.End + durationOffset,
        //  result.Text
        //);
      }
      if (start is not null && end is not null)
      {
        yield return new TranscriptionSegment(
          start.Value + durationOffset,
          RoundToTheClosestSecond(end.Value + durationOffset),
          sb.ToString()
        );
      }
    }
  }

  private static TimeSpan RoundToTheClosestSecond(TimeSpan timeSpan)
  {
    // Round to the nearest second
    return new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, 0);
  }

  private async IAsyncEnumerable<SegmentData> ProcessSegmentAsync(
    Stream segmentStream,
    [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    if (_whisperProcessor is null)
    {

      using var modelMemoryStream = new MemoryStream();
      var model = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.TinyEn, cancellationToken: cancellationToken).ConfigureAwait(false);
      await model.CopyToAsync(modelMemoryStream, cancellationToken).ConfigureAwait(false);
      var whisperFactory = WhisperFactory.FromBuffer(modelMemoryStream.ToArray());
      _whisperProcessor = whisperFactory.CreateBuilder()
        .WithLanguage("en")
        .Build();
    }

    await foreach (var result in _whisperProcessor.ProcessAsync(segmentStream, cancellationToken).ConfigureAwait(false))
    {
      yield return result;
    }
  }

  public void Dispose()
  {
    _whisperProcessor?.Dispose();
    _whisperProcessor = null;
  }
}