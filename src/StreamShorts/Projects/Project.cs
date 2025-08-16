using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

using StreamShorts.Library;

using Whisper.net;

namespace StreamShorts.Projects;
public class Project : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private string? _projectName;
	public string? ProjectName
	{
		get => _projectName;
		set
		{
			if (_projectName != value)
			{
				_projectName = value;
				OnPropertyChanged();
			}
		}
	}

	private Uri? _videoUri = null!;
	public Uri? VideoUri
	{
		get => _videoUri;
		set
		{
			if (_videoUri != value)
			{
				_videoUri = value;
				OnPropertyChanged();
			}
		}
	}

	private string? _localVideoPath;
	public string? LocalVideoPath
	{
		get => _localVideoPath;
		set
		{
			if (_localVideoPath != value)
			{
				_localVideoPath = value;
				OnPropertyChanged();
			}
		}
	}

	private string? _preview = null!;
	public string? Preview
	{
		get => _preview;
		set
		{
			if (_preview != value)
			{
				_preview = value;
				OnPropertyChanged();
			}
		}
	}

#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable CA1002 // Do not expose generic lists
	private ObservableCollection<ProjectFile> _files = [];
	public ObservableCollection<ProjectFile> Files
	{
		get => _files;
		set
		{
			if (_files != value)
			{
				_files = value;
				OnPropertyChanged();
			}
		}
	}
#pragma warning restore CA1002 // Do not expose generic lists
#pragma warning restore CA2227 // Collection properties should be read only

	private bool _isProcessing;
	public bool IsProcessing
	{
		get => _isProcessing;
		set
		{
			if (_isProcessing != value)
			{
				_isProcessing = value;
				OnPropertyChanged();
			}
		}
	}

	[JsonIgnore]
	public long TotalSizeInBytes
	{
		get
		{
			long fileLength = 0;

			var fileInfo = new FileInfo(Path.Combine(WorkingDirectory.Current, ProjectName!, ProjectName! + ".mp4"));
			if (fileInfo.Exists)
			{
				fileLength = fileInfo.Length;
			}
			return fileLength + Files.Where(f => f.SizeInBytes.HasValue).Sum(f => f.SizeInBytes ?? 0);
		}
	}
}
