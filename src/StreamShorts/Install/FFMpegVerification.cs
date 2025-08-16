using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamShorts.Install;
public static class FFMpegVerification
{
  public static async Task<bool> IsFFMpegInstalled()
  {
#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      using System.Diagnostics.Process process = new();
      process.StartInfo.FileName = "ffmpeg";
      process.StartInfo.Arguments = "-version";
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.Start();
      string output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
      string error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
      await process.WaitForExitAsync().ConfigureAwait(false);
      return output.Contains("ffmpeg version", StringComparison.InvariantCultureIgnoreCase);
    }
    catch 
    {
      return false;
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
}
