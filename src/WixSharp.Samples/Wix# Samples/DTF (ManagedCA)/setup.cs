using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using WixSharp;
using Microsoft.Win32;
using System;
using System.IO;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("CustomActionTest",
                new ManagedAction("MyAction", Return.check, When.After, Step.InstallInitialize, Condition.NOT_Installed));

        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        MessageBox.Show("Hello World!!!!!!!!!!!", "Embedded Managed CA");
        session.Log("Begin MyAction Hello World");

        return ActionResult.Success;
    }
}