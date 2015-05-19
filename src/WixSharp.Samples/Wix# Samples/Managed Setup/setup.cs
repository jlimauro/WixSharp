using Microsoft.Deployment.WindowsInstaller;

//css_ref ..\..\WixSharp.dll;
//css_ref ..\..\WixSharp.UI.dll;
//css_ref System.Core.dll;
//css_ref System.Xml.dll;
//css_ref ..\..\Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
using System.Windows.Forms;
using System.Xml.Linq;
using WixSharp;
using WixSharp.CommonTasks;

public class Script
{
    static public void Main()
    {
        //var ttt = ManagedProject.GetMsiForegroundWindow();
        new Script().Test();
    }

    static string ToMap(string wxlFile)
    {
        return null;
    }

    void Test()
    {
        //NOTE: IT IS STILL A WORK IN PROGRESS FEATURE PREVIEW
        var project =
            new ManagedProject("ManagedSetup",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File("readme.txt"))
                    //,
                    //new EmbeddedAssembly(HybridUI.Default.GetType().Assembly.Location)
                    //{
                    //    RefAssemblies = new[]
                    //    {
                    //        "%this%",
                    //        typeof(IEmbeddedUI).Assembly.Location 
                    //    }
                    //}
                    );

        //project.UI = WUI.WixUI_Mondo;

        project.EmbeddedUI = new EmbeddedAssembly(@"E:\Galos\Projects\WixSharp\src\EmbeddedUI_WPF\bin\Debug\EmbeddedUI_WPF.dll")
                    {
                        RefAssemblies = new[]
                        {
                            typeof(Session).Assembly.Location 
                        }
                    };
        //project.ManagedUI = ManagedUI.Default;

        //project.ManagedUI.BeforeInstall.Clear()
        //                               .Add<LicenceDialog>()
        //                               .Add<FeaturesDialog>()
        //                               .Add<InstallDirDialog>();

        //project.ManagedUI.AfterInstall.Clear()
        //                              .Add<ExitDialog>();

        //project.ManagedUI.BeforeRepair.Clear()
        //                              .Add<RepairStartDialog>();

        //project.ManagedUI.AfterRepair.Clear()
        //                             .Add<RepairExitDialog>();

        //project.ManagedUI.BeforeUninstall.Clear()
        //                                 .Add<UninstallStartDialog>();

        //project.ManagedUI.AfterUninstall.Clear()
        //                                .Add<UninstallExitDialog>();

        //project.ManagedUI = null;
        //project.UI = WUI.WixUI_Mondo;
        //project.AddBinary(new Binary("WixUI_en-us.wxl"));

        //project.UI = WUI.WixUI_ProgressOnly;
        //project.Load += project_Load;
        //project.BeforeInstall += project_BeforeExecute;
        //project.AfterInstall += project_AfterExecute;

#if vs
        project.OutDir = @"..\..\Wix# Samples\Managed Setup".PathGetFullPath();
#endif
        project.EmitConsistentPackageId = true;
        Compiler.CandleOptions += " -sw1091";

        project.Compiler.PreserveTempFiles = true;
        project.Compiler.WixSourceGenerated += Compiler_WixSourceGenerated;
        project.BuildMsiCmd();
    }

    void Compiler_WixSourceGenerated(XDocument document)
    {
        var product = document.Root.Select("Product");

        product.AddElement("UI")
               .AddElement("EmbeddedUI")
               .AddAttributes("Id=EmbeddedUI_WPF.dll;SourceFile=EmbeddedUI_WPF.CA.dll");

        product.Select("UIRef").Remove();
    }

    static void project_Load(SetupEventArgs e)
    {
        //Debugger.Launch();
        e.Result = ActionResult.UserExit;
    }

    static void project_BeforeExecute(SetupEventArgs e)
    {
        MessageBox.Show(e.ToString(), "BeforeInstall");
    }

    static void project_AfterExecute(SetupEventArgs e)
    {
        //Debugger.Launch();
        MessageBox.Show(e.ToString(), "AfterExecute");
    }
}
