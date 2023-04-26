using System;
using System.Diagnostics;
using System.IO;

namespace tScreen.Infra.Core.Shared
{
    public class Utility
    {
        public static string RunCommand(string command, bool throwWhenException = false)
        {
            try
            {
                var process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                process.Start();

                return process.StandardOutput.ReadToEnd();
            }
            catch (Exception)
            {
                if (throwWhenException) throw;
                return "";
            }
        }

        public static string GetGitHash()
        {
            return RunCommand("git rev-parse HEAD").TrimEnd();
        }

        public static string GetGitHashShort()
        {
            return GetGitHash()[..8];
        }
    }
}