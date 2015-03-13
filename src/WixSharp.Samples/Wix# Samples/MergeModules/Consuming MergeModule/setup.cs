//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;

using System;
using System.IO;
using File = WixSharp.File;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Feature featureA = new Feature("Feature A");
        Feature featureB = new Feature("Feature B");

        var MyBinariesFolder = "Files";
        var NUnitFolder = "NUnit";
        var myNUnitTestRunnerMergeFile = @"Files\MyMergeModule.msm";
        var LettersDiagnosticsSubFolder = "LettersDiagnosticsSubFolder";

        var project = new Project("SetupLettersDiagnostics",
                                  new Dir(@"%ProgramFiles%\test",
                                          new File(featureA,Path.Combine(NUnitFolder, "nunit.exe")),
                                          new File(featureA,Path.Combine(NUnitFolder, "Lib", "nunit-console-runner.dll"),
                                                   new Merge(featureB, myNUnitTestRunnerMergeFile))));
                
        // Project project =
               // new Project("MyMergeModule",
                   // new Dir(@"%ProgramFiles%\My Company",
                       // new File(featureA, @"Files\MainFile.txt"),	
                       // new Merge(featureB,@"Files\MyMergeModule.msm")));

        project.UI = WUI.WixUI_FeatureTree;

        Compiler.BuildMsi(project);
    }
}
