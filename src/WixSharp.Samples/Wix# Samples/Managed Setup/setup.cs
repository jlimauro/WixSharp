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
using WixSharp.Forms;
using WixSharp.UI.Forms;

public class Script
{
    static public void Main()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW

        var binaries = new Feature("Binaries", "Product binaries", true, false);
        var docs = new Feature("Documentation", "Product documentation (manuals and user guides)", true);
        var tuts = new Feature("Tutorials", "Product tutorials", true);
        docs.Children.Add(tuts);


        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, "myApp.exe"),
                    new Dir("Docs",
                        new File(docs, "readme.txt"),
                        new File(tuts, "setup.cs")))); 

        //project.LocalizationFile = "wixui_cs-cz.wxl";
        //project.Platform = Platform.x64;

        project.ManagedUI = ManagedUI.Default;
        project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                        .Add<LicenceDialog>()
                                        .Add<SetupTypeDialog>()
                                        .Add<FeaturesDialog>()
                                        .Add<InstallDirDialog>()
                                        .Add<ProgressDialog>()
                                        .Add<ExitDialog>();

        project.ManagedUI.ModifyDialogs.Add(Dialogs.SetupType)
                                       .Add<FeaturesDialog>()
                                       .Add<ProgressDialog>()
                                       .Add<ExitDialog>();

        project.Load += project_Load;
        project.BeforeInstall += project_BeforeExecute;
        project.AfterInstall += project_AfterExecute;

        //project.DefaultFeature = null;

#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        project.EmitConsistentPackageId = true;
        Compiler.CandleOptions += " -sw1091";

        //project.PreserveTempFiles = true;
        
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.BuildMsi();
    }

    static void project_Load(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Load");
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
