using System;
using System.Windows.Forms;

namespace WixSharp
{
    public class ClrDialogs
    {
        static Type LicenceDialog = typeof(LicenceDialog);
        static Type FeaturesDialog = typeof(FeaturesDialog);
        static Type InstallDirDialog = typeof(InstallDirDialog);
        static Type ExitDialog = typeof(ExitDialog);
        
        static Type RepairStartDialog = typeof(RepairStartDialog);
        static Type RepairExitDialog = typeof(RepairExitDialog);

        static Type UninstallStartDialog = typeof(UninstallStartDialog);
        static Type UninstallExitDialog = typeof(UninstallExitDialog);
    }

    public class LicenceDialog : Form, IManagedDialog
    {
        private TextBox sequenceView;

        private void InitializeComponent()
        {
            this.sequenceView = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // sequenceView
            //
            this.sequenceView.Location = new System.Drawing.Point(29, 28);
            this.sequenceView.Multiline = true;
            this.sequenceView.Name = "sequenceView";
            this.sequenceView.Size = new System.Drawing.Size(222, 192);
            this.sequenceView.TabIndex = 0;
            //
            // LicenceDialog
            //
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.sequenceView);
            this.Name = "LicenceDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}