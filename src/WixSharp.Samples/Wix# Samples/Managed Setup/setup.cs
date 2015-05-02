//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WixSharp;

public class Script
{
    static public void Main()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project = new ManagedProject("ManagedSetup");

        project.Load += project_Load;
        project.BeforeExecute += project_BeforeExecute;
        project.AfterExecute += project_AfterExecute;
        
        //project.DefaultRefAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

#if vs
        project.OutDir = @"..\..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        //Compiler.PreserveTempFiles = true;

        //System.Diagnostics.Debugger.Launch();

        Compiler.BuildMsi(project);
    }

    static void project_Load(SetupEventArgs e)
    {
        e.Session["test"] = "ttt";
        MessageBox.Show("Load");
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show("BeforeInstall", e.Session["test"]);
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        MessageBox.Show("AfterExecute");
    }
}

