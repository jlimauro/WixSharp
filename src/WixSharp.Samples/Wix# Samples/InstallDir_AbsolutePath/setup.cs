//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project = new Project("MyProduct",
                          new Dir(@"D:\MyCompany\MyProduct",
                              new Files(@"files\*.*")));

        project.UI = WUI.WixUI_ProgressOnly;

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }
}
