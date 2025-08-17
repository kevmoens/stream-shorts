using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using StreamShorts.MVVM.MVVM;
using System.IO;

#pragma warning disable CA1031 // Do not catch general exception types
namespace StreamShorts.MVVM.ViewModels;
public class InstallFFMpegViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public ICommand DropCommand { get; }
  public ICommand OpenHttpLinkCommand { get; }

  private string? _droppedZipFilePath;
  public string? DroppedZipFilePath
  {
    get => _droppedZipFilePath;
    set
    {
      if (_droppedZipFilePath != value)
      {
        _droppedZipFilePath = value;
        OnPropertyChanged();
      }
    }
  }

  public InstallFFMpegViewModel()
  {
    DropCommand = new DelegateCommand<DragEventArgs>(OnDrop);
    OpenHttpLinkCommand = new DelegateCommand(OpenFFMpegLink);
  }

  private void OpenFFMpegLink()
  {
    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
      FileName = "https://ffbinaries.com/downloads",
      UseShellExecute = true
    });
  }

  private void OnDrop(DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    {
      var files = (string[])e.Data.GetData(DataFormats.FileDrop);
      foreach (var zipFile in files)
      {
        if (zipFile.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == false)
        {
          continue; // Skip non-zip files
        }
        DroppedZipFilePath = zipFile;

        // Check if zip contains ffmpeg.exe
        var flowControl = ValidateZipFile(zipFile);
        if (!flowControl)
        {
          return;
        }

        // Extract zip to current working directory
        string extractPath = Environment.CurrentDirectory;
        try
        {
          System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractPath, overwriteFiles: true);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Failed to extract zip file: {ex.Message}", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      if (File.Exists(Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe")) == false)
      {
        MessageBox.Show("ffmpeg.exe was not found in the extracted files.", "Missing ffmpeg", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      if (File.Exists(Path.Combine(Environment.CurrentDirectory, "ffprobe.exe")) == false)
      {
        MessageBox.Show("ffprobe.exe was not found in the extracted files.", "Missing ffprobe", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      NavigationEvent.Instance.PublishEvent("ExistingProjects", []);

    }
  }

  private static bool ValidateZipFile(string zipFile)
  {
    try
    {
      using (var archive = System.IO.Compression.ZipFile.OpenRead(zipFile))
      {
        bool hasFfmpegExe = archive.Entries.Any(entry =>
          string.Equals(entry.Name, "ffmpeg.exe", StringComparison.OrdinalIgnoreCase)
          || string.Equals(entry.Name, "ffprobe.exe", StringComparison.OrdinalIgnoreCase));
        if (!hasFfmpegExe)
        {
          MessageBox.Show("The zip file does not contain ffmpeg.exe or ffprobe.exe.", "Missing ffmpeg and ffprobe", MessageBoxButton.OK, MessageBoxImage.Warning);
          return false;
        }
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Failed to read zip file: {ex.Message}", "Zip Error", MessageBoxButton.OK, MessageBoxImage.Error);
      return false;
    }

    return true;
  }
}
#pragma warning restore CA1031 // Do not catch general exception types