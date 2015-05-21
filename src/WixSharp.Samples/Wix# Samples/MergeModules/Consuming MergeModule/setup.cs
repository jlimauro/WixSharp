//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
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
        var featureA = new Feature("Feature A");
        var complete = new Feature("Complete");
        complete.Children.Add(featureA);

        var project =
                new Project("MyMergeModule",
                    new Dir(@"%ProgramFiles%\My Company",
                        new File(featureA, @"Files\MainFile.txt"),
                        new Merge(complete, @"Files\MyMergeModule.msm")));

        project.UI = WUI.WixUI_FeatureTree;

        Compiler.BuildMsi(project);
    }
}
