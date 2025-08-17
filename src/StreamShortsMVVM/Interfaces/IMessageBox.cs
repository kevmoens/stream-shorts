using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.Interfaces;
public interface IMessageBox
{
  MessageButtons Show(string message, string caption, MessageButtons buttons, MessageImage image);
}
