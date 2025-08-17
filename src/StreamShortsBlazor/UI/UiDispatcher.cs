
using Microsoft.AspNetCore.Components;

using StreamShorts.MVVM.Interfaces;

namespace StreamShortsBlazor.UI;

public class UiDispatcher : IUiDispatcher
{
  public Task InvokeAsync(Func<Task> action)
  {
    return Dispatcher.CreateDefault().InvokeAsync(action);
  }
}
