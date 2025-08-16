using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.Library;
public static class WorkingDirectory
{
  public static string Current { get; private set; }
  static WorkingDirectory()
  {
    Current = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StreamShorts");
  }
}
