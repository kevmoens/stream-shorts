using StreamShortsMVVM.Interfaces;

namespace StreamShortsBlazor.UI;

public class SettingsCanSave : ISettingsCanSave
{
  public bool CanSave()
  {
    return true;
  }
}
