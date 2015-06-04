//css_dir ..\..\..\;
//css_ref WixSharp.dll;
//css_ref System.Core.dll;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System;
using sys = System.Reflection;
//using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
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
        string productMsi = productProj.BuildMsi();

        //string productMsi = @"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Bootstrapper\WixBootstrapper_UI\My Product.msi";
        //---------------------------------------------------------

        var bootstrapper =
                new Bundle("My Product",
                    new PackageGroupRef("NetFx40Web"),
                    new MsiPackage(productMsi) { DisplayInternalUI = true });

        bootstrapper.Version = new Version("1.0.0.0");
        bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
        bootstrapper.Application = new ManagedBootstrapperApplication(@"..\ManagedBA\bin\Release\ManagedBA.dll");

        bootstrapper.PreserveTempFiles = true;
        bootstrapper.Build();
        //Process.Start("setup.exe");
    }

    //E:\temp\WiX\TestBA\BootstrapperSetup\Bundle.wxs
    //[assembly: BootstrapperApplication(typeof(CustomBootstrapperApplication))]

    //public class CustomBootstrapperApplication : BootstrapperApplication
    //{

    //    /// <summary>
    //    /// Entry point of managed code
    //    /// </summary>
    //    protected override void Run()
    //    {
    //        //System.Diagnostics.Debugger.Launch();

    //        this.Engine.Log(LogLevel.Verbose, "Running the TestBA.");
    //        MessageBox.Show("MBA is loaded");
    //        this.Engine.Quit(0);
    //        //... do your thing here 
    //    }
    //}
}

//public class BootstrapperViewModel
//{
//    public BootstrapperViewModel(BootstrapperApplication bootstrapper)
//    {
//        this.Bootstrapper = bootstrapper;
//        this.Bootstrapper.ApplyComplete += this.OnApplyComplete;
//        this.Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
//        this.Bootstrapper.PlanComplete += this.OnPlanComplete;
//    }

//    public bool CanInstall { get; set; }
//    public bool CanUninstall { get; set; } 
//    public bool IsBusy { get; set; } 

//    public BootstrapperApplication Bootstrapper { get; set; }

//    void InstallExecute()
//    {
//        IsBusy = true;
//        Bootstrapper.Engine.Plan(LaunchAction.Install);
//    }

//    void UninstallExecute()
//    {
//        IsBusy = true;
//        Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
//    }

//    /// <summary>
//    /// This is called after a bundle installation has completed. 
//    /// </summary>
//    private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
//    {
//        CanInstall = false;
//        CanUninstall = false;
//    }

//    /// <summary>
//    /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
//    /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
//    /// specified in one of the package elements (msipackage, exepackage, msppackage,
//    /// msupackage) in the WiX bundle.
//    /// </summary>
//    private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
//    {
//        if (e.PackageId == "DummyInstallationPackageId")
//        {
//            if (e.State == PackageState.Absent)
//                CanInstall = true;

//            else if (e.State == PackageState.Present)
//                CanUninstall = true;
//        }
//    }

//    /// <summary>
//    /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
//    /// If the planning was successful, it instructs the Bootstrapper Engine to 
//    /// install the packages.
//    /// </summary>
//    private void OnPlanComplete(object sender, PlanCompleteEventArgs e)
//    {
//        if (e.Status >= 0)
//            Bootstrapper.Engine.Apply(IntPtr.Zero);
//    }
//}

