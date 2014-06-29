//css_include ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {

        /* Use the presented coding style with caution. Because initializers are executed after the instantiation the intended 
         * execution order can easily be broken. Thus in the code fragment below the files will be assigned to the %ProgramFiles%
         * directory but not 'My Product'. 
         * 
         *     new Dir(@"%ProgramFiles%\My Company\My Product")
         *         {
         *             Files = new []
         *             {
         *                 new File(@"Files\Docs\Manual.txt"),
         *                 new File(@"Files\Bin\MyApp.exe")
         *             }
         *         }
         * 
         * This is because the directory path contains sub-directories, which are automatically wrapped into WixSharp.Dir
         * objects and assigned to the Dirs property of the parent directory object. From another hand initializer assigned files 
         * to the original root directory instead of appending them to the last Dir leaf.
         * 
         * Thus using constructors to build the hierarchy is a safe option. 
         */


        var project =   
            new Project()
            {
                Name = "MyProduct",
                GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b"),
                Dirs = new[]
                {
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                            new File(@"Files\Docs\Manual.txt"),
                            new File(@"Files\Bin\MyApp.exe"))
                }
            };

        Compiler.BuildMsiCmd(project);
    }
}
