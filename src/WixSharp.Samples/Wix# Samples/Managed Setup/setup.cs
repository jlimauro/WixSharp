//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project = new ManagedProject("ManagedSetup");

        project.Load += project_Load;
        project.BeforeInstall += project_BeforeExecute;
        project.AfterInstall += project_AfterExecute;
        //project.Exit += project_Exit;

        //project.DefaultRefAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

#if vs
        project.OutDir = @"..\..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        //Compiler.PreserveTempFiles = true;

        //System.Diagnostics.Debugger.Launch();

        //Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void project_Exit(SetupEventArgs e)
    {
        MessageBox.Show("Exit", GetContext());
    }

    static void project_Load(SetupEventArgs e)
    {
        e.Session["test"] = "ttt";
        MessageBox.Show("Load", GetContext());
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show("BeforeInstall " + e.Session["test"], GetContext());
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        MessageBox.Show("AfterExecute", GetContext());
    }

    static string GetContext()
    {
        if (WindowsIdentity.GetCurrent().IsAdmin())
            return "Admin User";
        else
            return "Normal User";
    }
}

