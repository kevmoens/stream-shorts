using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using StreamShorts.MVVM.Interfaces;

namespace StreamShortsBlazor.Pages;

public partial class MediaElement : IMediaElement
{
  [Parameter]
  public Uri? Source { get; set; }

  public async void Pause()
  {
    await JS!.InvokeVoidAsync("stopVideo").ConfigureAwait(false);
  }

  public async void Play()
  {
    await JS!.InvokeVoidAsync("playVideo").ConfigureAwait(false);
  }

  public async void Stop()
  {
    await JS!.InvokeVoidAsync("stopVideo").ConfigureAwait(false);
  }
}
