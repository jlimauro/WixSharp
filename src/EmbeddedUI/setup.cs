using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static void Main()
    {
        var project =
             new Project("EmbeddedUI_Setup",
                 new Dir(@"%ProgramFiles%\My Company\My Product",
                     new File("readme.txt")));

        project.EmbeddedUI = new EmbeddedAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location);

        project.Compiler.PreserveTempFiles = true;
        project.BuildMsi();
    }
}
