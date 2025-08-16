using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Helpers;

using StreamShorts.Library;

namespace StreamShorts.Projects;
public class ProjectFile
{
  public string Name { get; set; }
  public TimeSpan? Duration { get; set; }
  [JsonIgnore]
  public long? SizeInBytes { get; set; }
  public string? Description { get; set; }

  [JsonIgnore]
  public string Directory { get; set; }
  public ProjectFile()
  {
    Name = string.Empty;
    Directory = string.Empty;
  }
  public ProjectFile(Project project, string name) : this()
  {
    ArgumentNullException.ThrowIfNull(project);
    Directory = Path.Combine(WorkingDirectory.Current, project.ProjectName!);
    Name = name;
    var file = new FileInfo(Path.Combine(Directory, Name));
    SizeInBytes = file.Length;
  }
}

public static class ProjectFileExtensions
{

  public static async Task<ProjectFile> LoadDuration(this ProjectFile projectFile)
  {
#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      ArgumentNullException.ThrowIfNull(projectFile);
      projectFile.Duration = await GetVideoDurationAsync(Path.Combine(projectFile.Directory, projectFile.Name)).ConfigureAwait(false);
    }
    catch 
    {
      // Ignore if already configured
    }
#pragma warning restore CA1031 // Do not catch general exception types
    return projectFile;
  }

  public static async Task<TimeSpan?> GetVideoDurationAsync(string filePath)
  {
    if (!File.Exists(filePath))
      return null;

    var analysis = await FFProbe.AnalyseAsync(filePath).ConfigureAwait(false);
    return analysis.Duration;
  }
}
