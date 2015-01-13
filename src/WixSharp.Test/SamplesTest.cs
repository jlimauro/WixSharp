using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using IO = System.IO;

namespace WixSharp.Test
{
    public class SamplesTest
    {
        [Fact]
        public void CanBuildAllSamples()
        {
            var files = Directory.GetFiles(@"..\..\..\WixSharp.Samples\Wix# Samples", "build*.cmd", SearchOption.AllDirectories);

            var failedSamples = new List<string>();

            int startStep = 17;
            int currentStep = startStep;
            foreach (string file in files.Skip(startStep))
            {
                var batchFile = IO.Path.GetFullPath(file);

                try
                {
                    var dir = Path.GetDirectoryName(batchFile);

                    DeleteAllMsis(dir);
                    Assert.False(HasAnyMsis(dir), "Cannot clear directory for the test...");

                    DisablePause(batchFile);

                    string output = Run(batchFile);

                    if (output.Contains(" : error") || !HasAnyMsis(dir))
                        failedSamples.Add(batchFile);

                    DeleteAllMsis(dir);

                    Log(currentStep++, failedSamples);
                }
                catch (Exception e)
                {
                    failedSamples.Add(batchFile);
                    //Assert.True(false, e.Message);
                }
                finally
                {
                    RestorePause(batchFile);
                }
            }

            if (failedSamples.Any())
            {
                string error = "Failed Samples:\r\n" + string.Join(Environment.NewLine, failedSamples.ToArray());
                Assert.True(false, error);
            }

        }

        void Log(int currentStep, List<string> failedSamples)
        {
            var logFile = @"..\..\..\WixSharp.Samples\test_progress.txt";
            var content = string.Format("Failed Samples ({0}/{1}):\r\n", failedSamples.Count, currentStep + 1) + string.Join(Environment.NewLine, failedSamples.ToArray());
            IO.File.WriteAllText(logFile, content);
        }

        void DisablePause(string batchFile)
        {
            var batchContent = IO.File.ReadAllText(batchFile).Replace("\npause", "\nrem pause");
            IO.File.WriteAllText(batchFile, batchContent);
        }

        void RestorePause(string batchFile)
        {
            var batchContent = IO.File.ReadAllText(batchFile).Replace("\nrem pause", "\npause");
            IO.File.WriteAllText(batchFile, batchContent);
        }

        bool HasAnyMsis(string dir)
        {
            return Directory.GetFiles(dir, "*.msi").Any();
        }

        void DeleteAllMsis(string dir)
        {
            foreach (var msiFile in Directory.GetFiles(dir, "*.msi"))
                System.IO.File.Delete(msiFile);
        }

        string Run(string batchFile)
        {
            var process = new Process();
            process.StartInfo.FileName = batchFile;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(batchFile);
            process.Start();

            string line;
            var output = new StringBuilder();
            while (null != (line = process.StandardOutput.ReadLine()))
            {
                output.AppendLine(line);
            }

            process.WaitForExit();

            return output.ToString();
        }
    }
}