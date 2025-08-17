using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode.Videos.Streams;

namespace StreamShorts.MVVM.YouTube;

public class YouTubeDownload
{
  public Uri? URL { get; set; }
  public string? FolderPath { get; set; }

  public async Task<string> GetVideoName()
  {
    ArgumentNullException.ThrowIfNull(URL);
    var youtube = new YoutubeClient();

    var video = await youtube.Videos.GetAsync(URL.OriginalString).ConfigureAwait(false);

    var invalidChars = Path.GetInvalidFileNameChars();
    string videoTitle = string.Concat(video.Title.Where(c => !invalidChars.Contains(c)));

    return videoTitle;
  }
  public async Task<string> GetVideo()
  {
    ArgumentNullException.ThrowIfNull(URL, nameof(URL));
    ArgumentNullException.ThrowIfNull(FolderPath, nameof(FolderPath));

    var youtube = new YoutubeClient();
    var video = await youtube.Videos.GetAsync(URL.OriginalString).ConfigureAwait(false);

    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(URL.OriginalString).ConfigureAwait(false);


    // Select best audio stream (highest bitrate)
    var audioStreamInfo = streamManifest
      .GetAudioStreams()
      .Where(s => s.Container == Container.Mp4)
      .GetWithHighestBitrate();

    // Select best video stream (1080p60 in this example)
    var videoStreamInfo = streamManifest
      .GetVideoStreams()
      .Where(s => s.Container == Container.Mp4)
      .First(s => s.VideoQuality.Label == "1080p");

    // Download and mux streams into a single file
    var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };

    var invalidChars = Path.GetInvalidFileNameChars();
    string videoTitle = string.Concat(video.Title.Where(c => !invalidChars.Contains(c)));

    string path = Path.Combine(FolderPath, $"{videoTitle}.mp4");
    await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(path).Build()).ConfigureAwait(false);
    return path;

  }

	public async Task<byte[]> GetThumbnail()
	{
		ArgumentNullException.ThrowIfNull(URL, nameof(URL));
		var youtube = new YoutubeClient();
		var video = await youtube.Videos.GetAsync(URL.OriginalString).ConfigureAwait(false);

		// Get the highest resolution thumbnail
		var thumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).First().Url;

		// Download the thumbnail
		using var httpClient = new HttpClient();
		return await httpClient.GetByteArrayAsync(new Uri(thumbnailUrl)).ConfigureAwait(false);
	}
  public async Task<string> GetAudio()
  {
    ArgumentNullException.ThrowIfNull(URL, nameof(URL));
    ArgumentNullException.ThrowIfNull(FolderPath, nameof(FolderPath));
    var youtube = new YoutubeClient();
    var video = await youtube.Videos.GetAsync(URL.OriginalString).ConfigureAwait(false);

    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(URL.OriginalString).ConfigureAwait(false);

    var audioStreamInfo = streamManifest
      .GetAudioStreams()
      // Prefer audio streams in the default language (or non-language-specific streams)
      .OrderByDescending(s => s.IsAudioLanguageDefault ?? true)
      // Prefer audio streams with the same container
      .ThenByDescending(s => s.Container == Container.WebM)
      .ThenByDescending(s => s is AudioOnlyStreamInfo)
      .ThenByDescending(s => s.Bitrate)
      .FirstOrDefault();

    var trackInfos = new List<ClosedCaptionTrackInfo>();
    var streamInfos = new IStreamInfo[] { audioStreamInfo! };
    string path = Path.Combine(FolderPath, $"{video.Title}.mp3");
    await youtube.Videos.DownloadAsync(
      streamInfos,
      trackInfos,
      new ConversionRequestBuilder(path)
        .SetFFmpegPath("ffmpeg")
        .SetContainer(Container.Mp3)
        .SetPreset(ConversionPreset.Medium)
        .Build()
    ).ConfigureAwait(false);
    return path;

  }

}