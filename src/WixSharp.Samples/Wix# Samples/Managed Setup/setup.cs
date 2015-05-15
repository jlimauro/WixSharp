using Microsoft.Deployment.WindowsInstaller;
//css_ref ..\..\WixSharp.dll;
//css_ref ..\..\WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using WixSharp;

public class Script
{
    static public void Main()
    {
        //var ttt = ManagedProject.GetMsiForegroundWindow();
        new Script().Test();
    }

    void Test()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        project.UI = WUI.WixUI_Mondo;

        project.ManagedUI = ManagedUI.Default;

        project.ManagedUI.BeforeInstall.Clear()
                                       .Add<LicenceDialog>()
                                       .Add<FeaturesDialog>()
                                       .Add<InstallDirDialog>();

        project.ManagedUI.AfterInstall.Clear()
                                      .Add<ExitDialog>();

        project.ManagedUI.BeforeRepair.Clear()
                                      .Add<RepairStartDialog>();

        project.ManagedUI.AfterRepair.Clear()
                                     .Add<RepairExitDialog>();

        project.ManagedUI.BeforeUninstall.Clear()
                                         .Add<UninstallStartDialog>();

        project.ManagedUI.AfterUninstall.Clear()
                                        .Add<UninstallExitDialog>();




        //project.UI = WUI.WixUI_ProgressOnly;
        //project.Load += project_Load;
        //project.BeforeInstall += project_BeforeExecute;
        //project.AfterInstall += project_AfterExecute;


#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        project.EmitConsistentPackageId = true;
        Compiler.CandleOptions += " -sw1091";

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
        //Compiler.BuildWxs(project);
    }

    static void project_Load(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Load");
        e.Result = ActionResult.UserExit;
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}
