using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM;

/// <summary>
/// this class is a Publisher/Subscriber class
/// This class has a shared instance of itself
/// it has a method to publish an event
/// it has a method to subscribe to an event
/// </summary>
public class NavigationEvent : INavigationEvent
{
  private readonly List<Func<NavigationEventArgs, Task>> _navigationEvents = new List<Func<NavigationEventArgs, Task>>();
  public async Task PublishEvent(string page, Dictionary<string, object> parms)
  {
    foreach (var instance in _navigationEvents)
    {
      await instance.Invoke(new NavigationEventArgs(page, parms)).ConfigureAwait(false);
    }
  }
  public void SubscribeToEvent(Func<NavigationEventArgs, Task> handler)
  {
    _navigationEvents.Add(handler);
  }
  public void UnsubscribeToEvent(Func<NavigationEventArgs, Task> handler)
  {
    _navigationEvents.Remove(handler);
  }
}
