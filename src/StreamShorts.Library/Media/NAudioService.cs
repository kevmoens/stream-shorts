
using NAudio.Wave;

namespace StreamShorts.Library.Media;

/// <summary>
/// Represents a service for processing audio files using NAudio.
/// </summary>
/// <inheritdoc/>
internal sealed class NAudioService : IAudioService
{

  static void ConvertMp3ToWav16(string mp3)
  {
    Stream wavOutputStream = new MemoryStream();
    FFMpegCore.FFMpegArguments
        .FromFileInput(mp3)
        .OutputToFile(Path.ChangeExtension(mp3, ".wav"), true, (FFMpegCore.FFMpegArgumentOptions options) =>
        {
          options.WithAudioSamplingRate(16000); // Set the desired sample rate
          options.ForceFormat("wav");
        })
        .NotifyOnOutput(msg => Console.WriteLine(msg))
        .ProcessAsynchronously();
  }

  public Stream ConvertMp3ToWav16(Stream mp3)
  {
    return UseStream(mp3, stream =>
    {
      var tempMp3File = System.IO.Path.Combine(System.Environment.CurrentDirectory, "temp.mp3");
      var tempWavFile = System.IO.Path.ChangeExtension(tempMp3File, ".wav");
      try
      {
        //Write to temp file
        using (var mp3Stream = new FileStream(tempMp3File, FileMode.Create, FileAccess.Write))
        {
          stream.CopyTo(mp3Stream);
        }
        ConvertMp3ToWav16(tempMp3File);
        //Read WAV file
        var fileStream = new FileStream(tempWavFile, FileMode.Open, FileAccess.Read);
        return fileStream;
      }
      finally
      {
        //delete temp files
        if (System.IO.File.Exists(tempMp3File))
        {
          System.IO.File.Delete(tempMp3File);
        }
        if (System.IO.File.Exists(tempWavFile))
        {
          System.IO.File.Delete(tempWavFile);
        }
      }

      // Stream wavOutputStream = new MemoryStream();
      // FFMpegCore.FFMpegArguments
      //   .FromPipeInput(new FFMpegCore.Pipes.StreamPipeSource(mp3)) // Use StreamPipeSource for input stream
      //   .OutputToPipe(new FFMpegCore.Pipes.StreamPipeSink(wavOutputStream), options => options
      //       .WithAudioSamplingRate(16000) // Set the desired sample rate
      //   );
      // return wavOutputStream;
    });

    // return UseStream(mp3, stream =>
    // {
    //   using var reader = new Mp3FileReader(mp3);
    //   var outFormat = new WaveFormat(16000, reader.WaveFormat.Channels);
    //   using var resampler = new MediaFoundationResampler(reader, outFormat);
    //   var waveStream = new MemoryStream();
    //   WaveFileWriter.WriteWavFileToStream(waveStream, resampler);
    //   waveStream.Position = 0;
    //   return waveStream;
    // });
  }

  public int GetNumberOfWavSegments(Stream wavStream, TimeSpan segmentDuration)
  {
    return UseStream(wavStream, stream =>
    {
      var totalDuration = GetWavStreamDuration(wavStream);
      var segmentCount = (int)Math.Ceiling(totalDuration.TotalMilliseconds / segmentDuration.TotalMilliseconds);
      return segmentCount;
    });
    // return UseStream(wavStream, stream =>
    // {
    //   using var waveReader = new WaveFileReader(wavStream);
    //   var totalDuration = waveReader.TotalTime;
    //   var segmentCount = (int)Math.Ceiling(totalDuration.TotalMilliseconds / segmentDuration.TotalMilliseconds);
    //   return segmentCount;
    // });
  }

  public static TimeSpan GetWavStreamDuration(Stream wavStream)
  {
    // FFProbe needs a seekable stream to analyze its content effectively.
    // If your wavStream is not seekable (e.g., a network stream),
    // you should first copy its content to a MemoryStream or a temporary file.
    if (!wavStream.CanSeek)
    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
      Console.WriteLine("Warning: Input stream is not seekable. Copying to MemoryStream for analysis.");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
      MemoryStream tempStream = new MemoryStream();
      wavStream.CopyTo(tempStream);
      tempStream.Seek(0, SeekOrigin.Begin); // Reset position for analysis
      wavStream = tempStream;
    }

    try
    {
      wavStream.Seek(0, SeekOrigin.Begin); // Ensure stream is at the beginning for analysis
      string tempWaveFile = Path.Combine(System.Environment.CurrentDirectory, "temp.wav");
      using var fileStream = new FileStream(tempWaveFile, FileMode.Create, FileAccess.Write);
      wavStream.CopyTo(fileStream);
      fileStream.Seek(0, SeekOrigin.Begin);

      var mediaInfo = FFMpegCore.FFProbe.Analyse(tempWaveFile);
      File.Delete(tempWaveFile);
      return mediaInfo.Duration;
    }
    catch (FFMpegCore.Exceptions.FFProbeException ex)
    {
      Console.WriteLine($"FFProbe error analyzing WAV stream: {ex.Message}");
      return TimeSpan.Zero;
    }
  }
  public Stream GetWavSegment(Stream wavStream, int segmentNumber, TimeSpan segmentDuration)
  {
    return UseStream(wavStream, stream =>
    {
      using var segmentWaveReader = new WaveFileReader(wavStream);
      var segment = segmentWaveReader.ToSampleProvider()
        .Skip(segmentNumber * segmentDuration)
        .Take(segmentDuration);
      var segmentProvider = segment.ToWaveProvider16();
      var segmentStream = new MemoryStream();
      WaveFileWriter.WriteWavFileToStream(segmentStream, segmentProvider);
      segmentStream.Position = 0;
      return segmentStream;
    });
  }

  private static T UseStream<T>(Stream stream, Func<Stream, T> action)
  {
    ValidateStream(stream);

    var originalPosition = stream.Position;

    try
    {
      stream.Position = 0;
      return action(stream);
    }
    catch (System.Exception ex)
    {
      System.Console.WriteLine(ex.Message);
      throw;
    }
    finally
    {
      stream.Position = originalPosition;
    }
  }

  private static void ValidateStream(Stream stream)
  {
    if (stream == null)
    {
      throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null");
    }

    if (stream.CanRead is false)
    {
      throw new ArgumentException($"{nameof(stream)} must be readable", nameof(stream));
    }

    if (stream.CanSeek is false)
    {
      throw new ArgumentException($"{nameof(stream)} must be seekable", nameof(stream));
    }
  }
}