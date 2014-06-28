//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;

using System;
using WixSharp;
using Microsoft.Win32;

class Script
{
    static public void Main(string[] args)
    {
        var project = new Project("Setup",

           new LaunchCondition("NET20=\"#1\"", "Please install .NET 2.0 first."),

           new Dir(@"%ProgramFiles%\My Company\My Product",
                new File(@"Files\MyApp.exe")),

           new RegValueProperty("NET20", RegistryHive.LocalMachine, @"Software\Microsoft\NET Framework Setup\NDP\v2.0.50727", "Install", "0"));

        Compiler.BuildMsi(project);
    }
}






