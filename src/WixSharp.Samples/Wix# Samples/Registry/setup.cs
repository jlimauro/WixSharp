//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using Microsoft.Win32;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"readme.txt")),
                new RegValue(RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Message", "Hello"),
                new RegValue(RegistryHive.LocalMachine, "Software\\My Company\\My Product", "Count", 777));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.BuildMsi(project);
    }
}



