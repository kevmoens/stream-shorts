
using Microsoft.AspNetCore.Components;

using StreamShorts.MVVM.Interfaces;

namespace StreamShortsBlazor.UI;

public class UiDispatcher : IUiDispatcher
{
  public async Task InvokeAsync(Func<Task> action)
  {
    await Dispatcher.CreateDefault().InvokeAsync(action).ConfigureAwait(false);
  }
}
