using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.PC;

public partial class KeepPCAwake : IDisposable
{

  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
#pragma warning disable CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes
  private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
#pragma warning restore CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes

  [FlagsAttribute]
#pragma warning disable CA1028 // Enum Storage should be Int32
#pragma warning disable CA1707 // Identifiers should not contain underscores
  public enum EXECUTION_STATE : uint
  {
    ES_AWAYMODE_REQUIRED = 0x00000040,
    ES_CONTINUOUS = 0x80000000,
    ES_DISPLAY_REQUIRED = 0x00000002,
    ES_SYSTEM_REQUIRED = 0x00000001
    // Legacy flag, should not be used.
    // ES_USER_PRESENT = 0x00000004
  }
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1028 // Enum Storage should be Int32


  private Timer? _timer;

  public void Start()
  {
    _timer = new Timer(PreventSleepCallback, null, 0, 60000); // Call every 60 seconds
  }

  public void Stop()
  {
    _timer?.Dispose();
    _timer = null;
  }

  private void PreventSleepCallback(object? state)
  {
    // Prevent Idle-to-Sleep (monitor not affected) (see note above)
    SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      Stop();
    }
  }
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}