using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using StreamShorts.MVVM.MVVM;
using StreamShorts.MVVM.Projects;

using StreamShortsBlazor.UI;

namespace StreamShortsBlazor.Pages;

public partial class ProjectDetails
{
  private MessageBox? _messageBoxRef;
#pragma warning disable CA1805 // Do not initialize unnecessarily
#pragma warning disable IDE0044 // Add readonly modifier
  private MediaElement? _mediaElement = null;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CA1805 // Do not initialize unnecessarily
  [Inject]
  private NavigationPersistenceAware? _navigationPersistenceAware { get; set; }
  [Parameter] 
  public Guid? NavigationId { get; set; }
  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync().ConfigureAwait(false);
    ((MessageBoxWrapper)_viewModel.MessageBox).MessageBoxComponent = _messageBoxRef;

    _viewModel.PropertyChanged += (s,e) => StateHasChanged();
    if (NavigationId.HasValue && _viewModel is IPageNavigationAware navigationAware && _navigationPersistenceAware != null)
    {
      var parms = _navigationPersistenceAware.PickupNavigationParameters(NavigationId.Value);
      navigationAware.OnNavigatedTo(parms);
      StateHasChanged();
    }
  }

  public async Task OnSelectProjectFile(ProjectFile file)
  {
    if (file == null)
    {
      return;
    }
    string fileName = Path.Combine(file.Directory, file.Name);
    // Put the bytes in a memory stream
    using var stream = File.OpenRead(fileName);
    // Play the audio file
    using var streamRef = new DotNetStreamReference(stream: stream);
    await JS.InvokeVoidAsync("PlayAudioFileStream", streamRef).ConfigureAwait(false);
  }
  private async Task OnBack()
  {
    await _viewModel.OnBack().ConfigureAwait(false);
  }




}
