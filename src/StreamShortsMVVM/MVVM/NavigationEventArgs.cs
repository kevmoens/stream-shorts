using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM;

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
