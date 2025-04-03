using System;
using System.Diagnostics;
using System.Text;

namespace JetBrainsGuiApp
{
    public class ShellExecutor
    {
        public ProcessOutput ExecuteShellCommand(string command)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            var outputSegments = new ProcessOutput();

            if (string.IsNullOrWhiteSpace(command))
            {
                return new ProcessOutput(OutputType.AppError, "Command cannot be empty.");
            }

            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/c \"{command}\"";

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            process.StartInfo = startInfo;

            var standardOutput = string.Empty;
            var standardError = string.Empty;

            try
            {
                process.Start();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                bool exited = process.WaitForExit(2000);

                if (exited)
                {
                    standardOutput = outputTask.Result;
                    standardError = errorTask.Result;
                }
                else
                {
                    outputSegments.segment.Add(new Tuple<OutputType, string>(OutputType.AppError, "PROCESS TIMED OUT!"));

                    try { if (!process.HasExited) process.Kill(true); } catch {}

                    standardOutput = outputTask.Result ?? string.Empty;
                    standardError = errorTask.Result ?? string.Empty;

                    if (!string.IsNullOrEmpty(standardOutput)) 
                        outputSegments.segment.Add(new Tuple<OutputType, string>(OutputType.StandardOutput, $"--- Standard Output (Partial/Timeout) ---\n{standardOutput}"));
                    if (!string.IsNullOrEmpty(standardError)) 
                        outputSegments.segment.Add(new Tuple<OutputType, string>(OutputType.StandardError, $"--- Standard Error (Partial/Timeout) ---\n{standardError}"));
                    return outputSegments;
                }

            }
            catch (Exception ex)
            {
                return new ProcessOutput(OutputType.AppError, $"Failed to start process: {ex.Message}",process.ExitCode);
            }

            var text = new StringBuilder();

            if (standardOutput.Length > 0)
            {
                text.AppendLine("--- Standard Output ---");
                text.Append(standardOutput);

                outputSegments.segment.Add(new Tuple<OutputType, string>(OutputType.StandardOutput,text.ToString()));
                text.Clear();
            }
            if (standardError.Length > 0)
            {
                text.AppendLine("--- Standard Error ---");
                text.Append(standardError);

                outputSegments.segment.Add(new Tuple<OutputType, string>(OutputType.StandardError,text.ToString()));
            }

            outputSegments.ExitCode = process.ExitCode;
            return outputSegments;
        }
    }
}