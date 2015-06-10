//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
using System;
using System.Linq;
using System.Xml.Linq;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        var project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe")),
                new User(new Id("MyUser"), "James"),
                new User(new Id("MyOtherUser"), "James2") { WixIncludeInComponent = true },
                new SqlDatabase("MyDatabase", "localhost", 
                    new SqlString("alter login james with password = 'hi'", ExecuteSql.OnInstall)),
                new SqlDatabase("MyDatabase", "localhost",
                    new SqlString("alter login james with password = 'hey'", ExecuteSql.OnInstall)
                        { User = "MyOtherUser" })
                    {
                        CreateOnInstall = true,
                        User = "MyUser"
                    });

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.BuildMsi(project);
        Compiler.BuildWxs(project);
    }
}



