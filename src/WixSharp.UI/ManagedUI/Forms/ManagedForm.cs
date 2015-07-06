using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    public class ManagedForm : Form, IManagedDialog
    {
        public IManagedDialogContainer Shell { get; set; }

        public MsiRuntime MsiRuntime
        {
            get { return (MsiRuntime)Shell.RuntimeContext; }
        }

        virtual public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return MessageResult.OK;
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
    }
}
