//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        File notepad;

        var project = new Project("My Product",                  
                          new WixGuid("6fe30b47-2577-43ad-9095-1861ba25889b"),
                          new Dir(@"%ProgramFiles%\MyApp",
                              notepad = new File(@"C:\WINDOWS\system32\notepad.exe",
                                  new FileShortcut("Launch Notepad", @"%ProgramFiles%\MyApp") 
                                      { 
                                          Attributes = new Attributes() { { "Hotkey", "0" } } 
                                      })));

        notepad.Attributes = new Attributes { { "Component:SharedDllRefCount", "yes" } };

        var wxsFile = Compiler.BuildWxs(project);
    }
}



