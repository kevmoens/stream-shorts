using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamShorts.Projects.Processing;
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
public class VideoQueueManager : IDisposable, INotifyPropertyChanged
{

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private string? _message;

  public string? Message
  {
    get { return _message; }
    set { _message = value; OnPropertyChanged(); }
  }

  private readonly Queue<VideoWorkItem> _videoQueue = new();
  private readonly VideoWorkItemProcessor _processor;
  private readonly SemaphoreSlim _semaphore = new(1, 1);
  public VideoQueueManager(VideoWorkItemProcessor processor)
  {
    _processor = processor;
  }

  public void Dispose()
  {
    _semaphore.Dispose();
  }

  public async Task Enqueue(VideoWorkItem item)
  {
    _videoQueue.Enqueue(item);
    await ProcessNext().ConfigureAwait(false);
  }
  private async Task ProcessNext()
  {
    if (_videoQueue.Count == 0) return;
    await _semaphore.WaitAsync().ConfigureAwait(false);
    try
    {
      var item = _videoQueue.Dequeue();
      await foreach (var message in _processor.ProcessAsync(item).ConfigureAwait(false))
      {
        Message = message;
      }
    }
    finally
    {
      _semaphore.Release();
    }
    await ProcessNext().ConfigureAwait(false);
    if (Message?.StartsWith("Error", StringComparison.OrdinalIgnoreCase) ?? false)
    {
      return;
    }
    Message = string.Empty;
  }
}

#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly