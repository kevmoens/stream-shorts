using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM
{
  public interface IPage
  {
    string PageKey { get; }
    object? DataContext { get; set; }
  }
}
