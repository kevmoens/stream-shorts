using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StreamShorts.MVVM.MVVM;

namespace StreamShorts.MVVM.Install;
public static class OllamaVerification
{
#pragma warning disable CA1002 // Do not expose generic lists
  public static async Task<List<string>> GetModels()
#pragma warning restore CA1002 // Do not expose generic lists
  {
    try
    {
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
        FileName = "ollama",
        Arguments = "list",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };
      using (Process process = new Process())
      {
        process.StartInfo = startInfo;
        process.Start();
        using (StreamReader reader = process.StandardOutput)
        {
          string result = await reader.ReadToEndAsync().ConfigureAwait(false);
          return [.. result.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(line => line.Split([' '], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())];
        }
      }
    }
    catch (System.ComponentModel.Win32Exception)
    {
      //Ollama not installed
      await NavigationEvent.Instance.PublishEvent("InstallOllama", []).ConfigureAwait(false);
    }

    return new List<string>();

  }
}
