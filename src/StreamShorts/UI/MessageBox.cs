using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.UI;
public class MessageBox : IMessageBox
{
  public Task<MessageButtons> Show(string message, string caption, MessageButtons buttons, MessageImage image)
  {
    System.Windows.MessageBoxImage msgImage = image switch
    {
      MessageImage.Error => System.Windows.MessageBoxImage.Error,
      MessageImage.Warning => System.Windows.MessageBoxImage.Warning,
      _ => System.Windows.MessageBoxImage.None
    };
    System.Windows.MessageBoxButton msgButtons = buttons switch
    {
      MessageButtons.OK => System.Windows.MessageBoxButton.OK,
      MessageButtons.OKCancel => System.Windows.MessageBoxButton.OKCancel,
      MessageButtons.YesNo => System.Windows.MessageBoxButton.YesNo,
      _ => System.Windows.MessageBoxButton.OK
    };
    var result = System.Windows.MessageBox.Show(message, caption, msgButtons, msgImage);
    switch (result)
    {
      case System.Windows.MessageBoxResult.OK:
        return Task.FromResult(MessageButtons.OK);
      case System.Windows.MessageBoxResult.Cancel:
        return Task.FromResult(MessageButtons.Cancel);
      case System.Windows.MessageBoxResult.Yes:
        return Task.FromResult(MessageButtons.Yes);
      case System.Windows.MessageBoxResult.No:
        return Task.FromResult(MessageButtons.No);
      default:
        return Task.FromResult(MessageButtons.OK);
    }
  }
}
