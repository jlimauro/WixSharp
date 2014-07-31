using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Linq;
using ConsoleApplication1;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using System;

class CustomUIHelper
{
    /// <summary>
    /// Builds the custom UI.
    /// This is the equivalent of the CustomUIBuilder.BuildPostLicenseDialogUI implementation
    /// </summary>
    /// <returns></returns>
    public static CustomUI BuildCustomUI()
    {
        Dialog activationDialog = new ProductActivationForm().ToWDialog();

        XElement xml = activationDialog.ToXElement();

        var customUI = new CustomUI();

        customUI.CustomDialogs.Add(activationDialog);

        customUI.On(Dialogs.ExitDialog, Buttons.Finish, new CloseDialog() { Order = 9999 });

        customUI.On(Dialogs.WelcomeDlg, Buttons.Next, new ShowDialog(Dialogs.LicenseAgreementDlg));

        customUI.On(Dialogs.LicenseAgreementDlg, Buttons.Back, new ShowDialog(Dialogs.WelcomeDlg));
        customUI.On(Dialogs.LicenseAgreementDlg, Buttons.Next, new ShowDialog(activationDialog, "LicenseAccepted = \"1\""));

        customUI.On(activationDialog, Buttons.Back, new ShowDialog(Dialogs.LicenseAgreementDlg));

        customUI.On(activationDialog, Buttons.Next, new DialogAction { Name = "DoAction", Value = "ValidateLicenceKey" },
                                                    new ShowDialog(Dialogs.InstallDirDlg, "SERIALNUMBER_VALIDATED = \"TRUE\""));

        customUI.On(activationDialog, Buttons.Cancel, new CloseDialog("Exit"));

        customUI.On(Dialogs.InstallDirDlg, Buttons.Back, new ShowDialog(activationDialog));
        customUI.On(Dialogs.InstallDirDlg, Buttons.Next, new SetTargetPath(),
                                                         new ShowDialog(Dialogs.VerifyReadyDlg));

        customUI.On(Dialogs.InstallDirDlg, Buttons.ChangeFolder,
                                                         new SetProperty("_BrowseProperty", "[WIXUI_INSTALLDIR]"),
                                                         new ShowDialog(CommonDialogs.BrowseDlg));

        customUI.On(Dialogs.VerifyReadyDlg, Buttons.Back, new ShowDialog(Dialogs.InstallDirDlg, Condition.NOT_Installed),
                                                          new ShowDialog(Dialogs.MaintenanceTypeDlg, Condition.Installed));

        customUI.On(Dialogs.MaintenanceWelcomeDlg, Buttons.Next, new ShowDialog(Dialogs.MaintenanceTypeDlg));

        customUI.On(Dialogs.MaintenanceTypeDlg, Buttons.Back, new ShowDialog(Dialogs.MaintenanceWelcomeDlg));
        customUI.On(Dialogs.MaintenanceTypeDlg, Buttons.Repair, new ShowDialog(Dialogs.VerifyReadyDlg));
        customUI.On(Dialogs.MaintenanceTypeDlg, Buttons.Remove, new ShowDialog(Dialogs.VerifyReadyDlg));

        return customUI;
    }
}
