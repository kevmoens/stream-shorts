using StreamShorts.MVVM.Interfaces;
using StreamShorts.MVVM.Projects;

namespace StreamShortsBlazor.UI;

public class ProjectFileMediaElement : IProjectFileMediaElement
{
  public ProjectFileMediaElement(ProjectFile file, IMediaElement mediaElement)
  {
    ProjectFile = file;
    MediaElement = mediaElement;
  }
  public ProjectFile ProjectFile { get; set; }
  public IMediaElement MediaElement { get; set; }
}
