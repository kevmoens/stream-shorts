
using StreamShorts.MVVM.Interfaces;

using StreamShortsBlazor.Pages;

namespace StreamShortsBlazor.UI;

public class MessageBoxWrapper : IMessageBox
{
  public MessageBox? MessageBoxComponent { get; set; }
  public async Task<MessageButtons> Show(string message, string caption, MessageButtons buttons, MessageImage image)
  {
    if (MessageBoxComponent != null)
    {
      return await MessageBoxComponent.Show(message, caption, buttons, image).ConfigureAwait(false);
    }
    throw new InvalidOperationException("MessageBoxComponent is not set.");
  }
}
