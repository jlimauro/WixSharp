//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using sys = System.Reflection;
using WixSharp;
using WixSharp.Bootstrapper;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.Diagnostics;

public class InstallScript
{
    static public void Main(string[] args)
    {
        var productProj =
            new Project("My Product",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt"))) { InstallScope = InstallScope.perUser };
        
        productProj.GUID = new Guid("6f330b47-2577-43ad-9095-1861bb258777");
        string productMsi = productProj.BuildMsi();

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application = new SilentBootstrapperApplication();

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.Build();
    }
}
