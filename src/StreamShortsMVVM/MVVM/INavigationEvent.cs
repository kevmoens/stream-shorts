using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StreamShorts.MVVM.MVVM;
public interface INavigationEvent
{
  Task PublishEvent(string page, Dictionary<string, object> parms);
  void SubscribeToEvent(Func<NavigationEventArgs, Task> handler);
  void UnsubscribeToEvent(Func<NavigationEventArgs, Task> handler);
}
