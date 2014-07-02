//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main()
    {
        Project project =
            new Project("MyProduct",
                new Dir(new Id("PRODUCT_INSTALLDIR"), @"%ProgramFiles%\My Company\My Product",
                    new File(new Id("App_File"), @"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(new Id("Manual_File"), @"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }

    static public void AlternativeSyntax()
    {
        Project project =
            new Project("MyProduct",
                new Dir {
                    Id = "PRODUCT_INSTALLDIR",
                    Name = @"%ProgramFiles%\My Company\My Product",
                    
                    Files = new[] { new File { Id = "App_File", Name = @"Files\Bin\MyApp.exe" } },

                    Dirs = new[] { new Dir { 
                                        Name = @"Docs\Manual",
                                        Files = new[] { new File { Id="Manual_File", Name=@"Files\Docs\Manual.txt" } } } } });

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
    }
}



