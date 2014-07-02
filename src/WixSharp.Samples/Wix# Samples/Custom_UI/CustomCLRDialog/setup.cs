//css_inc CustomDialog.cs
using System;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System.Xml;
using IO = System.IO;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Drawing;

class Script
{
    static public void Main()
    {
        ManagedAction customDialog;

        var project = new Project("CustomDialogTest",
                                    customDialog = new ShowClrDialogAction("ShowProductActivationDialog"));

        project.UI = WUI.WixUI_Common;
        project.CustomUI = CustomUIBuilder.InjectPostLicenseClrDialog(customDialog.Id, " LicenseAccepted = \"1\"");
        
        Compiler.PreserveTempFiles = true; //comment if you don't want to investigate generated *.wxs
        Compiler.BuildMsi(project);
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult ShowProductActivationDialog(Session session)
    {
        return WixCLRDialog.ShowAsMsiDialog(new CustomDialog(session));
    }
}