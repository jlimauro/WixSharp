using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharp.Forms;
using WixSharp.UI.Forms;

public class Script
{
    static public void Main()
    {
        Tasks.GetInstalledProducts(); return;

        var binaries = new Feature("Binaries", "Product binaries", true, false);
        var docs = new Feature("Documentation", "Product documentation (manuals and user guides)", true);
        var tuts = new Feature("Tutorials", "Product tutorials", false);
        docs.Children.Add(tuts);

        var project = new ManagedProject("ManagedSetup",
                            new Dir(@"%ProgramFiles%\My Company\My Product",
                                new File(binaries, @"..\Files\bin\MyApp.exe"),
                                new Dir("Docs",
                                    new File(docs, "readme.txt"),
                                    new File(tuts, "setup.cs"))));

        project.ManagedUI = new ManagedUI();

        project.UIInitialized += UIInitialized;

        //removing all entry dialogs and installdir
        project.ManagedUI.InstallDialogs//.Add(Dialogs.Welcome)
                                        //.Add(Dialogs.Licence)
                                        //.Add(Dialogs.SetupType)
                                        .Add(Dialogs.Features)
                                        //.Add(Dialogs.InstallDir)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        //removing entry dialog
        project.ManagedUI.ModifyDialogs//.Add(Dialogs.MaintenanceType)
                                        .Add(Dialogs.Features)
                                        .Add(Dialogs.Progress)
                                        .Add(Dialogs.Exit);

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        project.SourceBaseDir = @"..\..\";

        project.BuildMsi();
    }

    private static void UIInitialized(SetupEventArgs e)
    {
        if (e.IsInstalling)
        {
            MessageBox.Show(e.ToString(), "Before UI");
        }
    }
}
