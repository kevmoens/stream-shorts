using Microsoft.AspNetCore.Components;

using StreamShortsBlazor.UI;

namespace StreamShortsBlazor.Pages;

public partial class Index 
{

  private MessageBox? _messageBoxRef;

  protected override async Task OnInitializedAsync()
  {
    await _viewModel.OnLoaded().ConfigureAwait(false);
    await base.OnInitializedAsync().ConfigureAwait(false);
    ((MessageBoxWrapper)_viewModel.MessageBox).MessageBoxComponent = _messageBoxRef;
  }

  private async Task OnSettingsClick()
  {
    await _viewModel.OnSettings().ConfigureAwait(false);
  }
  private async Task OnAddProjectClick()
  {
    await _viewModel.OnAddProject().ConfigureAwait(false);
  }
  private async Task OnEditProjectClick(StreamShorts.MVVM.Projects.Project project)
  {
    await _viewModel.OnOpenProject(project).ConfigureAwait(false);
  }
  private async Task OnDeleteProjectClick(StreamShorts.MVVM.Projects.Project project)
  {
    await _viewModel.OnDeleteProject(project).ConfigureAwait(false);
  }

}
