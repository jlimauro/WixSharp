//css_ref ..\..\WixSharp.dll;
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
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt")));

        project.UI = WUI.WixUI_ProgressOnly;

        project.Load += project_Load;
        project.BeforeInstall += project_BeforeExecute;
        project.AfterInstall += project_AfterExecute;

#if vs
        project.OutDir = @"..\..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif

        Compiler.BuildMsi(project);
    }

    static void project_Load(SetupEventArgs e)
    {
        ManagedUI.HideWindow(e.MsiWindow);
        MessageBox.Show(e.ToString(), "Load");
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
        ManagedUI.ShowWindow(e.MsiWindow);
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        ManagedUI.HideWindow(e.MsiWindow);
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}

public class ManagedUI
{
    public static void HideWindow(IntPtr wnd)
    {
        ShowWindow(wnd, SW_HIDE);
    }
   
    public static void ShowWindow(IntPtr wnd)
    {
        ShowWindow(wnd, SW_SHOW);
    }

    const int SW_HIDE = 0;
    const int SW_SHOW = 1;

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
