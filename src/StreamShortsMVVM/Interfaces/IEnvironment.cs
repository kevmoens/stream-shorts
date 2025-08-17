using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShortsMVVM.Interfaces;
public interface IEnvironment
{
  Environments Environment { get; set; }
}
