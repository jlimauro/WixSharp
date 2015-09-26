using System;
using System.Windows.Forms;
using WixSharp.UI.Forms;

namespace WixSharp.Forms
{
    /// <summary>
    /// Set of ManagedUI dialogs (WinForm) that implement all standard MSI UI dialogs.
    /// </summary>
    public class Dialogs
    {
        /// <summary>
        /// The standard Welcome dialog
        /// </summary>
        static public Type Welcome = typeof(WelcomeDialog);
        /// <summary>
        /// The standard Licence dialog
        /// </summary>
        static public Type Licence = typeof(LicenceDialog);
        /// <summary>
        /// The standard Features dialog
        /// </summary>
        static public Type Features = typeof(FeaturesDialog);
        /// <summary>
        /// The standard InstallDir dialog
        /// </summary>
        static public Type InstallDir = typeof(InstallDirDialog);
        /// <summary>
        /// The standard Installation Progress dialog
        /// </summary>
        static public Type Progress = typeof(ProgressDialog);
        /// <summary>
        /// The standard Setup Type dialog. To be used during the installation of the product.
        /// </summary>
        static public Type SetupType = typeof(SetupTypeDialog);
        /// <summary>
        /// The standard Maintenance Type dialog.To be used during the maintenance of the product.
        /// </summary>
        static public Type MaintenanceType = typeof(MaintenanceTypeDialog);
        /// <summary>
        /// The standard Exit dialog
        /// </summary>
        static public Type Exit = typeof(ExitDialog);
    }

    class ShellView : Form
    {
        public ShellView()
        {
            InitializeComponent();
            this.Load += (sender, e) => TopMost = false; //To ensure initial 'on top'
        }

        void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 361);
            //this.Size = new System.Drawing.Size(510, 400);
            this.MinimumSize = new System.Drawing.Size(510, 400);
            this.MaximumSize = new System.Drawing.Size(510, 400);
            this.MaximizeBox = false;
            this.Name = "UIShell";
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Icon = ".msi".GetAssiciatedIcon();
            this.ResumeLayout(false);
        }
    }
}