using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    public class ManagedForm : Form, IManagedDialog
    {
        IManagedUIShell shell;
        
        public IManagedUIShell Shell
        {
            get { return shell; }
        
            set
            {
                shell = value;
                OnShellChanged();
            }
        }

        public MsiRuntime MsiRuntime
        {
            get { return (MsiRuntime)Shell.RuntimeContext; }
        }

        virtual public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return MessageResult.OK;
        }

        virtual public void OnShellChanged()
        {
        }

        virtual public void OnExecuteComplete()
        {
        }

        virtual public void OnExecuteStarted()
        {
        }

        virtual public void OnProgress(int progressPercentage)
        {
        }

        public void Localize()
        {
            this.LocalizeFrom(MsiRuntime.Localize);
        }
    }
}
