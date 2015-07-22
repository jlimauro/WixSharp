using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The base class for all WinForm based dialogs of ManagedUI.
    /// </summary>
    public class ManagedForm : Form, IManagedDialog
    {
        IManagedUIShell shell;

        /// <summary>
        /// Gets or sets the UI shell (main UI window). This property is set the ManagedUI runtime (IManagedUI). 
        /// On the other hand it is consumed (accessed) by the UI dialog (IManagedDialog).
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public IManagedUIShell Shell
        {
            get { return shell; }
        
            set
            {
                shell = value;
                OnShellChanged();
            }
        }

        /// <summary>
        /// Gets the MSI runtime context.
        /// </summary>
        /// <value>
        /// The msi runtime.
        /// </value>
        public MsiRuntime MsiRuntime
        {
            get { return (MsiRuntime)Shell.RuntimeContext; }
        }

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage" />.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        virtual public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return MessageResult.OK;
        }

        /// <summary>
        /// Called when Shell is changed. It is a good place to initialize the dialog to reflect the MSI session 
        /// (e.g. localize the view).
        /// </summary>
        virtual protected void OnShellChanged()
        {
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        virtual public void OnExecuteComplete()
        {
        }

        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        virtual public void OnExecuteStarted()
        {
        }

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        virtual public void OnProgress(int progressPercentage)
        {
        }

        /// <summary>
        /// Localizes the form and its contained <see cref="T:System.Windows.Forms.Control.Text"/> from the specified localization 
        /// delegate 'localize'. 
        /// <para>The method substitutes both localization file (*.wxl) entries and MSI properties contained by the input string
        /// with their translated/converted values.</para>
        /// <remarks>
        /// Note that both localization entries and MSI properties must be enclosed in the square brackets 
        /// (e.g. "[ProductName] Setup", "[InstallDirDlg_Title]").
        /// </remarks>/// </summary>
        public void Localize()
        {
            this.LocalizeWith(MsiRuntime.Localize);
        }
    }
}
