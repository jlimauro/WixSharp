//css_dir ..\..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;
using WixSharp.UI.Forms;

public class Script
{
    static public void Main()
    {
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"..\Files\bin\MyApp.exe"),
                    new Dir("Docs",
                        new File("readme.txt"))));

        project.ManagedUI = ManagedUI.Empty;

        project.Load += project_Load;
        project.BeforeInstall += project_BeforeInstall;
        project.AfterInstall += project_AfterInstall;

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }

    static void project_Load(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "Load");
    }

    static void project_BeforeInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
    }

    static void project_AfterInstall(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}
