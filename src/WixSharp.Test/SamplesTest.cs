using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;
using IO = System.IO;

namespace WixSharp.Test
{
    public class SamplesTest
    {
        string[] nonMsiProjects = new[] { "CustomAttributes" };

        [Fact]
        public void CanBuildAllSamples()
        {
            var failedSamples = new List<string>();
            int startStep = 0;
            int currentStep = startStep;

            var files = Directory.GetFiles(@"..\..\..\WixSharp.Samples\Wix# Samples", "build*.cmd", SearchOption.AllDirectories);
            foreach (string file in files.Skip(startStep))
            {
                currentStep++;
                var batchFile = IO.Path.GetFullPath(file);
                BuildSample(batchFile, currentStep, failedSamples);
            }

            if (failedSamples.Any())
            {
                string error = "Failed Samples:\r\n" + string.Join(Environment.NewLine, failedSamples.ToArray());
                Assert.True(false, error);
            }
        }

        void BuildSample(string batchFile, int currentStep, List<string> failedSamples)
        {
            try
            {
                var dir = Path.GetDirectoryName(batchFile);

                DeleteAllMsis(dir);
                Assert.False(HasAnyMsis(dir), "Cannot clear directory for the test...");

                DisablePause(batchFile);

                string output = Run(batchFile);

                bool nonMsi = !nonMsiProjects.Where(x => batchFile.Contains(x)).Any();

                if (output.Contains(" : error") || (nonMsi && !HasAnyMsis(dir)))
                    failedSamples.Add(currentStep + ":" + batchFile);

                DeleteAllMsis(dir);

                Log(currentStep, failedSamples);
            }
            catch (Exception e)
            {
                failedSamples.Add(currentStep + ":" + batchFile + "\t" + e.Message.Replace("\r\n", "\n").Replace("\n", ""));
            }
            finally
            {
                RestorePause(batchFile);
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
            return Directory.GetFiles(dir, "*.ms?").Any();
        }

        void DeleteAllMsis(string dir)
        {
            foreach (var msiFile in Directory.GetFiles(dir, "*.ms?"))
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