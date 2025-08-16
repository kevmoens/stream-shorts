using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StreamShorts.Projects;
public class ProjectFileMediaElement
{
  public ProjectFileMediaElement(ProjectFile projectFile, MediaElement mediaElement)
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
  private MediaElement _mediaElement;

  public MediaElement MediaElement
  {
    get { return _mediaElement; }
    set { _mediaElement = value; }
  }

}
