using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.MVVM;
public interface IPageNavigationAware
{
  void OnNavigatedTo(Dictionary<string, object> parameters);
}
