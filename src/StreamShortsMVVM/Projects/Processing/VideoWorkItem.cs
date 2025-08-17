using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.Projects.Processing;
public class VideoWorkItem
{
  public Project? Project { get; set; }
  public CancellationTokenSource Cancellation { get; set; } = new();

}
