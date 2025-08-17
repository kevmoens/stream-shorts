using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using StreamShorts.MVVM.Interfaces;
using StreamShorts.MVVM.Projects;

namespace StreamShorts.Projects;
public class ProjectFileMediaElement : IProjectFileMediaElement
{
  public ProjectFileMediaElement(ProjectFile projectFile, IMediaElement mediaElement)
  {
    _projectFile = projectFile;
    _mediaElement = mediaElement;
  }
  private ProjectFile _projectFile;

  public ProjectFile  ProjectFile
  {
    get { return _projectFile; }
    set { _projectFile = value; }
  }
  private IMediaElement _mediaElement;

  public IMediaElement MediaElement
  {
    get { return _mediaElement; }
    set { _mediaElement = value; }
  }

}
