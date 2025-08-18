using StreamShortsBlazor.UI;

namespace StreamShortsBlazor.Pages;

public partial class AddProject
{
  private MessageBox? _messageBoxRef;
  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync().ConfigureAwait(false);
    _viewModel.PropertyChanged += async (s, e) => await InvokeAsync(() => StateHasChanged()).ConfigureAwait(false);
  }
  protected override void OnAfterRender(bool firstRender)
  {
    base.OnAfterRender(firstRender);
    ((MessageBoxWrapper)_viewModel.MessageBox).MessageBoxComponent = _messageBoxRef;
  }
  public async Task OnAddProject()
  {
    await _viewModel.OnAddProject().ConfigureAwait(false);
    await _viewModel.OnValidateYouTubeUrl().ConfigureAwait(false);
  }
}
