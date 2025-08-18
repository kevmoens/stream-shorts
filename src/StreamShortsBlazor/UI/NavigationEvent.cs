
using Microsoft.AspNetCore.Components;

using StreamShorts.MVVM.MVVM;

namespace StreamShortsBlazor.UI;

public class NavigationEvent : INavigationEvent
{
  private readonly NavigationManager _navigationManager;
  private readonly NavigationPersistenceAware _navigationPersistenceAware;

  public NavigationEvent(NavigationManager navigationManager, NavigationPersistenceAware navigationPersistenceAware)
  {
    _navigationManager = navigationManager;
    _navigationPersistenceAware = navigationPersistenceAware;
  }

  public Task PublishEvent(string page, Dictionary<string, object> parms)
  {
    if (parms != null && parms.Count > 0)
    {
      var newId = _navigationPersistenceAware.QueueNavigationParameters(parms);
      _navigationManager.NavigateTo(page + $"/{newId}");
      return Task.CompletedTask;
    }
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
