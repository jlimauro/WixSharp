//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Feature common = new Feature("Common Files");
        Feature samples = new Feature("Samples");

        Project project =
            new Project("MyProduct",
                new Dir(common, @"%ProgramFiles%\My Company\My Product",
                    new Dir(common, @"Docs\Manual"),
                    new Dir(samples, @"Samples")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_FeatureTree;

        Compiler.BuildMsi(project);
    }
}



