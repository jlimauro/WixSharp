using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp
{
    public interface IManagedDialog
    {
        IManagedUIShell Shell { get; set; }
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
        void OnExecuteComplete();
        void OnExecuteStarted();
        void OnProgress(int progressPercentage);
    }

    public interface IManagedUIShell
    {
        object RuntimeContext { get; }
        string Log { get; }
        bool UserInterrupted { get; }
        bool ErrorDetected { get;}

        ManagedDialogs Dialogs { get; }

        void GoNext();
        void GoPrev();
        void GoTo(int index);
        void Cancel();
        void Exit();
        void StartExecute();
    }

    public interface IManagedUI
    {
        ManagedDialogs InstallDialogs { get; }
        ManagedDialogs ModifyDialogs { get; }
        void EmbeddResourcesInto(ManagedProject project);
    }

    public class ManagedDialogs : List<Type>
    {
        public ManagedDialogs Add<T>() where T : IManagedDialog
        {
            base.Add(typeof(T));
            return this;
        }

        public new ManagedDialogs Add(Type type)
        {
            base.Add(type);
            return this;
        }

        public new ManagedDialogs Clear()
        {
            base.Clear();
            return this;
        }
    }
}
