using Microsoft.Deployment.WindowsInstaller;
//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

public class Script
{
    static public void Main()
    {
        new Script().Test();
    }

    static string ToMap(string wxlFile)
    {
        return null;
    }

    void Test()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW

        var binaries = new Feature("Binaries", true, false);
        var docs = new Feature("Documentation", true);
        var tuts = new Feature("Tutorials", true);
        binaries.Children.Add(docs);

        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, "myApp.exe"),
                    new Dir("Docs",
                        new File(docs, "readme.txt"),
                        new File(tuts, "setup.cs")))); 

        project.UI = WUI.WixUI_Mondo;

        //project.EmbeddedUI = new EmbeddedAssembly(@"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Custom_UI\EmbeddedUI_WPF\bin\Debug\EmbeddedUI_WPF.dll");
        //project.EmbeddedUI = new EmbeddedAssembly(@"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Custom_UI\EmbeddedUI\bin\Debug\EmbeddedUI.exe");
        //project.LocalizationFile = "wixui_cs-cz.wxl";
        //project.LicenceFile = "License.rtf";
        //project.Platform = Platform.x64;


        project.ManagedUI = ManagedUI.Default;
        project.ManagedUI.InstallDialogs.Clear()
                                        .Add<WelcomeDialog>()
                                        .Add<LicenceDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<InstallDirDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();

        project.ManagedUI.ModifyDialogs.Clear()
                                       //.Add<ModifyStartDialog>()
                                        .Add<FeaturesDialog>()
                                       .Add<ProgressDialog>()
                                       .Add<ExitDialog>();


        //project.ManagedUI = null;
        //project.UI = WUI.WixUI_FeatureTree;

        //project.Load += project_Load;
        //project.BeforeInstall += project_BeforeExecute;
        //project.AfterInstall += project_AfterExecute;

#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        project.EmitConsistentPackageId = true;
        Compiler.CandleOptions += " -sw1091";

        project.PreserveTempFiles = true;
        
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.BuildMsi();
    }

    static void project_Load(SetupEventArgs e)
    {
        //Debugger.Launch();
        //e.Result = ActionResult.UserExit;
        MessageBox.Show(e.ToString(), "Load");
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
        //e.Result = ActionResult.Failure;
        //e.Result = ActionResult.UserExit;
        //e.Result = Microsoft.Deployment.WindowsInstaller.ActionResult.UserExit;
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        //Debugger.Launch();
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}
