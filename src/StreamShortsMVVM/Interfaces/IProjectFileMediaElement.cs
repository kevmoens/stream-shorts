using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamShorts.MVVM.Projects;

namespace StreamShorts.MVVM.Interfaces;
public interface IProjectFileMediaElement
{
  ProjectFile ProjectFile { get; set; }
  IMediaElement MediaElement { get; set; } 
}
