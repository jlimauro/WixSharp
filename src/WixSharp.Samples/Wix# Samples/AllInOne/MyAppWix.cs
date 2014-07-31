//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using System;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        try
        {
            Feature binaries = new Feature("MyApp Binaries");
            Feature docs = new Feature("MyApp Documentation");

            Project project =
                new Project("My Product",

                    //Files and Shortcuts
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        new File(binaries, @"AppFiles\MyApp.exe",
                            new FileShortcut(binaries, "MyApp", @"%ProgramMenu%\My Company\My Product"),
                            new FileShortcut(binaries, "MyApp", @"%Desktop%")),
                        new File(binaries, @"AppFiles\Registrator.exe"),
                        new File(docs, @"AppFiles\Readme.txt"),
                        new File(binaries, @"AppFiles\MyApp.ico"),
                        new ExeFileShortcut(binaries, "Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")),

                    new Dir(@"%ProgramMenu%\My Company\My Product",
                        new ExeFileShortcut(binaries, "Uninstall MyApp", "[System64Folder]msiexec.exe", "/x [ProductCode]")),

                    //Rigistries
                    new RegValue(binaries, RegistryHive.LocalMachine, @"Software\My Product", "ExePath", @"[INSTALLDIR]MyApp.exe"),

                    //Custom Actions
                    new InstalledFileAction("Registrator.exe", "", Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                    new InstalledFileAction("Registrator.exe", "/u", Return.check, When.Before, Step.InstallFinalize, Condition.Installed),

                    new ScriptAction(@"MsgBox ""Executing VBScript code...""", Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),

                    new ScriptFileAction(@"CustomActions\Sample.vbs", "Execute", Return.ignore, When.After, Step.InstallFinalize, Condition.NOT_Installed),

                    new PathFileAction(@"%WindowsFolder%\notepad.exe", "readme.txt", @"INSTALLDIR", Return.asyncNoWait, When.After, Step.InstallFinalize, Condition.NOT_Installed),

                    new ManagedAction(@"MyManagedAction", "%this%"),

                    new InstalledFileAction("MyApp.exe", ""));


            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b"); // or project.Id = Guid.NewGuid();   
            project.LicenceFile = @"AppFiles\License.rtf";
            project.UI = WUI.WixUI_Mondo;
            project.SourceBaseDir = Environment.CurrentDirectory;
            project.MSIFileName = "MyApp";

            //Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class CustomActions
{
    [CustomAction]
    public static ActionResult MyManagedAction(Session session)
    {
        MessageBox.Show("Hello World!", "Embedded Managed CA");

        return ActionResult.Success;
    }
}

