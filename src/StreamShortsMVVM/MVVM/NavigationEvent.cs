using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM
{
    /// <summary>
    /// this class is a Publisher/Subscriber class
    /// This class has a shared instance of itself
    /// it has a method to publish an event
    /// it has a method to subscribe to an event
    /// </summary>
    public class NavigationEvent
    {
        private readonly static NavigationEvent _instance = new NavigationEvent();
        public static NavigationEvent Instance
        {
            get
            {
                return _instance;
            }
        }
        public event EventHandler<NavigationEventArgs>? NavigationEventOccurred;
        public void PublishEvent(string page, Dictionary<string, object> parms)
        {
            NavigationEventOccurred?.Invoke(this, new NavigationEventArgs(page, parms));
        }
        public void SubscribeToEvent(EventHandler<NavigationEventArgs> handler)
        {
            NavigationEventOccurred += handler;
        }
        public void UnsubscribeToEvent(EventHandler<NavigationEventArgs> handler)
        {
            NavigationEventOccurred -= handler;
        }
    }

  public class NavigationEventArgs(string page, Dictionary<string, object> arms) : EventArgs
  {
    public string Page { get; } = page;
    public Dictionary<string, object> Parms { get; } = arms;
    public NavigationEventArgs(string page) : this(page, new Dictionary<string, object>())
    {
    }
    public NavigationEventArgs() : this(string.Empty, new Dictionary<string, object>())
    {
    }
  }
}
