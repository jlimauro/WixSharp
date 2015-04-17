//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Linq;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var feat = new Feature("Main");
        var project =
            new Project("MyProduct",
                new Dir(feat, @"%ProgramFiles%\My Company\My Product",
                    new Files(feat, @"Release\*.*"),
                              //f => !f.EndsWith(".obj") &&
                                //   !f.EndsWith(".pdb")),
                    new ExeFileShortcut(feat, "Uninstall My Product", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1561ba25889b");

        project.ResolveWildCards(ignoreEmptyDirectories:false)
               .FindFile((f) => f.Name.EndsWith("MyApp.exe"))
               .First()
               .Shortcuts = new[] {
                                       new FileShortcut(feat, "MyApp.exe", "INSTALLDIR"),
                                       new FileShortcut(feat, "MyApp.exe", "%Desktop%")
                                  };

        Compiler.BuildMsiCmd(project);
    }
}
