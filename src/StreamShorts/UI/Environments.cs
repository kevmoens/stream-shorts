using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamShortsMVVM.Interfaces;

namespace StreamShorts.UI;
public class Environments : IEnvironment
{
  public StreamShortsMVVM.Interfaces.Environments Environment { get; set; } = StreamShortsMVVM.Interfaces.Environments.WPF;
}
