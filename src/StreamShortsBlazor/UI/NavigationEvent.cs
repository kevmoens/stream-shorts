
using Microsoft.AspNetCore.Components;

using StreamShorts.MVVM.MVVM;

namespace StreamShortsBlazor.UI;

public class NavigationEvent : INavigationEvent
{
  private readonly NavigationManager _navigationManager;

  public NavigationEvent(NavigationManager navigationManager)
  {
    _navigationManager = navigationManager;
  }
  public Task PublishEvent(string page, Dictionary<string, object> parms)
  {
    _navigationManager.NavigateTo(page);
    return Task.CompletedTask;
  }

  public void SubscribeToEvent(Func<NavigationEventArgs, Task> handler)
  {
    //Not needed in Blazor
  }

  public void UnsubscribeToEvent(Func<NavigationEventArgs, Task> handler)
  {
    //Not needed in Blazor    
  }
}
