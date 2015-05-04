//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Diagnostics;
using System.Linq;
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
        var project = new ManagedProject("ManagedSetup");

        project.UI = WUI.WixUI_ProgressOnly;

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
        MessageBox.Show("Exit", GetContext(e));
    }

    static void project_Load(SetupEventArgs e)
    {
        IntPtr msiWindow = IntPtr.Zero;
        try
        {
            msiWindow = FindWindow(null, "ManagedSetup");
            msiWindow = GetMsiForegroundWindow();
        }
        catch { }

        ShowWindow(msiWindow, SW_HIDE);
       
        e.Session["WIXSHARP_RUNTIME_DATA"] =
            string.Format("Installed: {0}\nREMOVE: {1}\nUILevel: {2}\nMsiWindow: {3}",
            e.Session["Installed"], e.Session["REMOVE"], e.Session["UILevel"], msiWindow);
        MessageBox.Show(GetContext(e), "Load");
    }

    const int SW_HIDE = 0;
    const int SW_SHOW = 1;

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    static IntPtr GetMsiWindow(SetupEventArgs e)
    {
        try
        {
            var value = e.Session .Property("WIXSHARP_RUNTIME_DATA")
                                  .Split('\n')
                                  .First(x => x.StartsWith("MsiWindow: "))
                                  .Replace("MsiWindow: ", "");
            return (IntPtr)int.Parse(value);
        }
        catch { }
        return IntPtr.Zero;
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show(GetContext(e), "BeforeInstall");
        ShowWindow(GetMsiWindow(e), SW_SHOW);
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        ShowWindow(GetMsiWindow(e), SW_HIDE);
        MessageBox.Show(GetContext(e), "AfterExecute");
    }

    static string GetContext(SetupEventArgs e)
    {
        var result = new StringBuilder();

        if (WindowsIdentity.GetCurrent().IsAdmin())
            result.AppendLine("Executing as 'Admin User'");
        else
            result.AppendLine("Executing as 'Normal User'");

        result.AppendLine(e.Session.Property("WIXSHARP_RUNTIME_DATA"));
        return result.ToString();
    }

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    static IntPtr GetMsiForegroundWindow()
    {
        var proc = Process.GetProcessesByName("msiexec").Where(p => p.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
        if (proc != null)
        {
            Win32.ShowWindow(proc.MainWindowHandle, Win32.SW_RESTORE);
            Win32.SetForegroundWindow(proc.MainWindowHandle);
            return proc.MainWindowHandle;
        }
        else return IntPtr.Zero;
    }
}

