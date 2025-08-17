using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using StreamShorts.Library;
using StreamShorts.MVVM.Interfaces;

namespace StreamShorts.MVVM.Projects;
public class ProjectFolderRepo
{
  private readonly IUiDispatcher _uiDispatcher;
  public ProjectFolderRepo(IUiDispatcher uiDispatcher) 
  {
    _uiDispatcher = uiDispatcher;
  }
  private readonly ObservableCollection<Project> _projects = [];
  public ObservableCollection<Project> Projects { get { return _projects; } }
  public async Task LoadAll()
  {
    if (Projects.Count > 0)
    {
      return; // Already loaded
    }
    DirectoryInfo dir = new(WorkingDirectory.Current);
    _projects.Clear();
    if (!dir.Exists)
    {
      dir.Create();
      return;
    }

    foreach (DirectoryInfo subDir in dir.GetDirectories())
    {
      // Read in directory json file with descriptions and other metadata if exists
      if (File.Exists(Path.Combine(subDir.FullName, "details.json")) is false)
      {
        continue; // Skip directories without details.json
      }
      Project? loadProject = null;
      using (FileStream detailStream = File.OpenRead(Path.Combine(subDir.FullName, "details.json")))
      {
        loadProject = await JsonSerializer.DeserializeAsync<Project>(detailStream).ConfigureAwait(false);
      }
      if (loadProject is null)
      {
        continue; // Skip if deserialization failed
      }
      YouTube.YouTubeDownload yt = new();
      yt.URL = loadProject.VideoUri;
      // Align date from json with actual files
      var files = await Task.WhenAll(
        subDir.GetFiles()
        .Where(f => string.Equals(f.Name, loadProject.ProjectName + ".mp4", StringComparison.OrdinalIgnoreCase) == false && f.Name != "details.json" && f.Name != "preview.jpg")
        .Select(async f =>
          await new ProjectFile(loadProject, f.Name)
            .LoadDuration()
            .ConfigureAwait(false)
          )
        ).ConfigureAwait(false);
      if (loadProject.Files == null)
      {
        loadProject.Files = new ObservableCollection<ProjectFile>();
      }
      foreach (var file in files)
      {
        if (loadProject.Files.Any(f => string.Equals(f.Name, file.Name, StringComparison.OrdinalIgnoreCase)))
        {
          // If file already exists in the project, skip it
          continue;
        }
        ProjectFile? newfile = file;
        if (newfile.Duration is null)
        {
          // If duration is not set, try to load it
          newfile = await newfile.LoadDuration().ConfigureAwait(false);
        }

        loadProject.Files.Add(newfile);
      }
      foreach (var file in loadProject.Files)
      {
        file.Directory = subDir.FullName;
        if (file.Duration is null)
        {
          // If duration is not set, try to load it
          var newFile = await file.LoadDuration().ConfigureAwait(false);
          file.Duration = newFile.Duration;
        }
        if (file.SizeInBytes is null)
        {
          // If size is not set, set it
          var fileInfo = new FileInfo(Path.Combine(file.Directory, file.Name));
          if (fileInfo.Exists)
          {
            file.SizeInBytes = fileInfo.Length;
          }
        }
      }
      loadProject.Preview = "preview.jpg";

      await _uiDispatcher.InvokeAsync(() =>
      {
        _projects.Add(loadProject);
        return Task.CompletedTask;
      }).ConfigureAwait(false);
    }
    return;
  }

  public async Task AddProject(Project project)
  {
    ArgumentNullException.ThrowIfNull(project);

    await _uiDispatcher.InvokeAsync(() =>
    {
      Projects.Add(project);
      return Task.CompletedTask;
    }).ConfigureAwait(false);
  }
  public async Task DeleteProject(Project project)
  {
    ArgumentNullException.ThrowIfNull(project);

    DirectoryInfo dir = new(Path.Combine(WorkingDirectory.Current, project.ProjectName!));
    dir.Delete(true); // Delete the directory and all its contents

    await _uiDispatcher.InvokeAsync(() =>
    {
      Projects.Remove(project);
      return Task.CompletedTask;
    }).ConfigureAwait(false);
  }
}
