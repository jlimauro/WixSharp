//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main()
    {
        var project =
            new Project("MyProduct",
                new Dir(@"C:\My Company\MyProduct",
                    new File("readme.txt")));

        project.UI = WUI.WixUI_InstallDir;
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}
