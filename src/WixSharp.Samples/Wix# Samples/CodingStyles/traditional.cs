//css_include ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        /* Use the presented coding style with caution. Because property assignments are executed after the instantiation the 
         * intended execution order can easily be broken. Thus in the code fragment below the files will be assigned to the 
         * %ProgramFiles% directory but not 'My Product'. 
         * 
         *  var dir = new Dir(@"%ProgramFiles%\My Company\My Product");
         *  dir.Files = new[] { docFile, exeFile };
         *  project.Dirs = new[] { dir };
         * 
         * This is because the directory path contains sub-directories, which are automatically wrapped into WixSharp.Dir
         * objects and assigned to the Dirs property of the parent directory object. From another hand dir.Files are assigned  
         * to the original root directory instead of appending them to the last Dir leaf.
         * 
         * Thus using constructors to build the hierarchy is a safe option. 
         */

        var docFile = new File(@"Files\Docs\Manual.txt");
        var exeFile = new File(@"Files\Bin\MyApp.exe");
        var dir = new Dir("My Product");
        dir.Files = new[] { docFile, exeFile };

        var project = new Project();
        project.Dirs = new[] { new Dir("%ProgramFiles%", 
                                   new Dir("My Company", 
                                       dir)) };

        project.Name = "MyProduct";
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        
        Compiler.BuildMsiCmd(project);
    }
}
