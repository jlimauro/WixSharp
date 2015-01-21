//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref  ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project()
        {
            UI = WUI.WixUI_InstallDir,
            Name = "CustomActionTest",

            Actions = new WixSharp.Action[]{
                            new SetPropertyAction("IDIR", "[INSTALLDIR]"),
                            new ManagedAction(@"MyAction")},
                            
            Properties = new[] {
                            new Property("IDIR", "empty") ,
                            new Property("Test", "empty") },

            Dirs = new[] { 
                            new Dir(@"%ProgramFiles%\CustomActionTest"){
                                    Files = new []{
                                        new File("readme.txt")}}}
        };

        Compiler.BuildWxs(project);
    }
}

public class CustonActions
{
    [CustomAction]
    public static ActionResult MyAction(Session session)
    {
        try
        {
            MessageBox.Show(session["IDIR"], "InstallDir (INSTALLDIR copy)");
            MessageBox.Show(session["INSTALLDIR"], "InstallDir (actual INSTALLDIR)");
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), "Error");
        }
        return ActionResult.Success;
    }
}

