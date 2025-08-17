using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.MVVM.Interfaces;
public interface IMediaElement
{
  void Play();
  void Pause();
#pragma warning disable CA1716 // Identifiers should not match keywords
  void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
}
