using System.Collections.Concurrent;

namespace StreamShortsBlazor.UI;

public class NavigationPersistenceAware
{
  private readonly ConcurrentDictionary<Guid, Dictionary<string, object>> _persistentParms = [];
  public Guid QueueNavigationParameters(Dictionary<string, object> parms)
  {
    Guid newId = Guid.NewGuid();
    if (_persistentParms.TryAdd(newId, parms))
    { 
      return newId; 
    }
    throw new InvalidOperationException("Unable setup navigation parameters");
  }
  public Dictionary<string, object> PickupNavigationParameters(Guid id)
  {
    if (_persistentParms.TryRemove(id, out var parms))
    {
      return parms;
    }
    throw new KeyNotFoundException("No navigation parameters found for the given ID");
  }
}
