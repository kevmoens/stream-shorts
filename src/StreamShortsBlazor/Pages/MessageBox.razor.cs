using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using StreamShorts.MVVM.Interfaces;

namespace StreamShortsBlazor.Pages;

public partial class MessageBox
{
  [Parameter] public string Message { get; set; } = "This is a default message.";
  [Parameter] public string AlertType { get; set; } = "info"; // Options: primary, success, danger, etc.
  [Parameter] public bool IsVisible { get; set; } = false;
  private TaskCompletionSource<MessageButtons>? _taskCompletionSource;
  private string AlertClass => $"alert-{AlertType}";
  private MessageButtons _visibleButtons;
#pragma warning disable IDE0044 // Add readonly modifier
  private MessageButtons _result = MessageButtons.OK;
#pragma warning restore IDE0044 // Add readonly modifier

  public void OnButtonClick(MessageButtons button)
  {
    _result = button;
    if (_taskCompletionSource != null)
    {
      _taskCompletionSource.SetResult(button);
    }
    IsVisible = false;
    StateHasChanged();
  }
  public async Task<MessageButtons> Show(string message, string caption, MessageButtons buttons, MessageImage image)
  {
    _taskCompletionSource = new TaskCompletionSource<MessageButtons>();
    Message = message;
    _visibleButtons = buttons;
    switch (image)
    {
      case MessageImage.None:
        AlertType = "info";
        break;
      case MessageImage.Error:
        AlertType = "danger";
        break;
      case MessageImage.Warning:
        AlertType = "warning";
        break;
      default:
        AlertType = "info";
        break;
    }
    IsVisible = true;
    await InvokeAsync(()=> StateHasChanged()).ConfigureAwait(false);
    await _taskCompletionSource.Task.ConfigureAwait(false);
    _taskCompletionSource = null;
    return _result;
  }
}
