//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;

using WixSharp.CommonTasks;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

using System.Security.Principal;

using System.Diagnostics;

//http://avinashkt.blogspot.com.au/2007/05/how-to-install-windows-service-using.html

class Script
{
    static public void Main(string[] args)
    {
        try
        {
            File service;
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\My Company\My Product",
                        service = new File(@"SimpleService\bin\Debug\MyApp.exe")));

            service.ServiceInstaller = new ServiceInstaller
                                       {
                                           Name = "WixSharp.TestSvc",
                                           StartOn = SvcEvent.Install,
                                           StopOn = SvcEvent.InstallUninstall_Wait,
                                           RemoveOn = SvcEvent.Uninstall_Wait,
                                       };

            project.GUID = new Guid("6fe30b47-2577-43ad-9195-1861ba25889b");
            project.OutFileName = "setup";
            project.SourceBaseDir = @"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\WinService";

            Compiler.BuildMsiCmd(project);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
