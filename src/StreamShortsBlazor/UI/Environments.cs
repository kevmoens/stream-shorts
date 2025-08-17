using StreamShortsMVVM.Interfaces;

namespace StreamShortsBlazor.UI;

public class Environments : IEnvironment
{
  public StreamShortsMVVM.Interfaces.Environments Environment { get; set; } = StreamShortsMVVM.Interfaces.Environments.Blazor;
}