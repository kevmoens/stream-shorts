using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.UI;
public class UiDispatcher : IUiDispatcher
{
  public Task InvokeAsync(Func<Task> action)
  {
    System.Windows.Application.Current.Dispatcher.Invoke(async () => await action.Invoke().ConfigureAwait(false));
    return Task.CompletedTask;
  }
}
