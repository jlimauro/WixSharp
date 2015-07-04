using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp
{
    public interface IManagedDialog
    {
        IManagedDialogContainer Shell { get; set; }
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
        void OnExecuteComplete();
        void OnExecuteStarted();
        void OnProgress(int progressPercentage);
    }

    public interface IManagedDialogContainer
    {
        object RuntimeContext { get; }
        string Log { get; }

        void GoNext();
        void GoPrev();
        void Cancel();
        void Exit();
        void StartExecute();
    }

    public interface IManagedUI
    {
        ManagedDialogs InstallDialogs { get; }
        ManagedDialogs RepairDialogs { get; }
    }

    public class ManagedDialogs : List<Type>
    {
        public ManagedDialogs Add<T>() where T : IManagedDialog
        {
            base.Add(typeof(T));
            return this;
        }

        public new ManagedDialogs Clear()
        {
            base.Clear();
            return this;
        }
    }
}
