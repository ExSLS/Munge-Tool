using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MungeTool.Lib.Tests.Helpers
{
    public static class ProcessHelper
    {
        public static void StartProcess(string command, string arguments, string workingDirectory = null,
            StringBuilder stdoutBuffer = null, StringBuilder stderrBuffer = null)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                if(workingDirectory != null)
                    process.StartInfo.WorkingDirectory = workingDirectory;

                stdoutBuffer = stdoutBuffer ?? new StringBuilder();
                stderrBuffer = stderrBuffer ?? new StringBuilder();

                using (var outputWaitHandle = new AutoResetEvent(false))
                {
                    using (var errorWaitHandle = new AutoResetEvent(false))
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                                // ReSharper disable once AccessToDisposedClosure
                                outputWaitHandle.Set();
                            else
                                stdoutBuffer.AppendLine(e.Data);
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                                // ReSharper disable once AccessToDisposedClosure
                                errorWaitHandle.Set();
                            else
                                stderrBuffer.AppendLine(e.Data);
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        var timeout = (int) TimeSpan.FromMinutes(1).TotalMilliseconds;

                        if (process.WaitForExit(timeout) &&
                            outputWaitHandle.WaitOne(timeout) &&
                            errorWaitHandle.WaitOne(timeout))
                        {
                            Console.WriteLine(stdoutBuffer);
                            Console.WriteLine(stderrBuffer);

                            if (process.ExitCode != 0)
                                throw new Exception($"Exit code for {command} {arguments} was {process.ExitCode}\n\nStdout:\n{stdoutBuffer}\n\n{stderrBuffer}");
                        }
                        else
                        {
                            throw new Exception("Operation timed out");
                        }
                    }
                }
            }
        }
    }
}
