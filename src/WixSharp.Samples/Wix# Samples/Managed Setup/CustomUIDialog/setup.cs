using System;
using System.Linq;
using System.Windows.Forms;
using WixSharp;
using Wix_Forms = WixSharp.UI.Forms;

internal static class Defaults
{
    public const string UserName = "MP_USER";
}

public class Script
{

    static public void Main()
    {
        var project = new ManagedProject("ManagedSetup",
                          new User
                          {
                              Name = Defaults.UserName,
                              Password = "[PASSWORD]",
                              Domain = "[DOMAIN]",
                              PasswordNeverExpires = true,
                              CreateUser = true
                          },
                          new Property("PASSWORD", "pwd123"));

        project.SourceBaseDir = @"..\..\";
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
        project.LocalizationFile = "MyProduct.en-us.wxl";

        project.ManagedUI = new ManagedUI();
        project.ManagedUI.InstallDialogs.Add<Wix_Forms.WelcomeDialog>()
                                        .Add<MyProduct.UserNameDialog>()
                                        .Add<Wix_Forms.ProgressDialog>()
                                        .Add<Wix_Forms.ExitDialog>();

        //it effectively becomes a 'Repair' sequence 
        project.ManagedUI.ModifyDialogs.Add<Wix_Forms.ProgressDialog>()
                                       .Add<Wix_Forms.ExitDialog>();

        project.BeforeInstall += msi_BeforeInstall;

        project.BuildMsi();
    }

    static void msi_BeforeInstall(SetupEventArgs e)
    {
        //Note: the property will not be from UserNameDialog if MSI UI is suppressed
        if (e.Session["DOMAIN"] == null)
            e.Session["DOMAIN"] = Environment.MachineName;
    }
}
