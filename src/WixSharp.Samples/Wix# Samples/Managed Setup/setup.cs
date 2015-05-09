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
        var project = new Project("MyProduct",
                          new Dir(@"D:\MyCompany\MyProduct",
                          //new Dir(@"%ProgramFiles%\My Company\My Product",
                              new Files(@"files\*.*")));

        project.UI = WUI.WixUI_ProgressOnly;
#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsiCmd(project);

        //new Script().Test();
    }

    void Test()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        project.UI = WUI.WixUI_ProgressOnly;
        project.EmitConsistentPackageId = true;

        project.Load += project_Load;
        project.BeforeInstall += project_BeforeExecute;
        project.AfterInstall += project_AfterExecute;

        //ManagedUI.AttachTo(project);

#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
        //Compiler.BuildWxs(project);
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
