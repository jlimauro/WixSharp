//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System.Windows.Forms;
using System.Xml.Linq;
using System;
using System.Linq;
using WixSharp;
using WixSharp.CommonTasks;
using Microsoft.Deployment.WindowsInstaller;

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
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        //project.UI = WUI.WixUI_Mondo;

        //project.EmbeddedUI = new EmbeddedAssembly(@"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Custom_UI\EmbeddedUI_WPF\bin\Debug\EmbeddedUI_WPF.dll");
        //project.EmbeddedUI = new EmbeddedAssembly(@"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Custom_UI\EmbeddedUI\bin\Debug\EmbeddedUI.exe");
        project.ManagedUI = ManagedUI.Default;

        project.ManagedUI.InstallDialogs.Clear()
                                        .Add<LicenceDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<InstallDirDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();
 
        project.ManagedUI.RepairDialogs.Clear()
                                       .Add<RepairStartDialog>()
                                       .Add<ProgressDialog>()
                                       .Add<RepairExitDialog>();


        //project.ManagedUI = null;
        //project.UI = WUI.WixUI_Mondo;

        //project.Load += project_Load;
        //project.BeforeInstall += project_BeforeExecute;
        //project.AfterInstall += project_AfterExecute;

#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        project.EmitConsistentPackageId = true;
        Compiler.CandleOptions += " -sw1091";

        project.PreserveTempFiles = true;
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
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        //Debugger.Launch();
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}
