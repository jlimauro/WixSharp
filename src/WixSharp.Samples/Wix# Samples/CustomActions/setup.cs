//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project()
        {
            Name = "CustomActionTest",
            UI = WUI.WixUI_ProgressOnly,

            Dirs = new[]
            { 
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Registrator.exe"))
            },

            Actions = new WixSharp.Action[] 
            { 	
                //execute installed appication
                new InstalledFileAction(@"Registrator.exe", "/u", Return.check, When.Before, Step.InstallFinalize, Condition.Installed),
                new InstalledFileAction(@"Registrator.exe", "", Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed),

                //execute existing application
                new PathFileAction(@"%WindowsFolder%\notepad.exe", @"C:\boot.ini", "INSTALLDIR", Return.asyncNoWait, When.After, Step.PreviousAction, Condition.NOT_Installed),

                //execute VBS code
                new ScriptAction(@"MsgBox ""Executing VBScript code...""", Return.ignore, When.After, Step.PreviousAction, Condition.NOT_Installed),
                
                //execute embedded VBS file
                new ScriptFileAction(@"Files\Sample.vbs", "Execute" , Return.ignore, When.After, Step.PreviousAction, Condition.NOT_Installed)
            }
        };

        var file = Compiler.BuildMsi(project);
    }
}



