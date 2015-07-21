using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp
{
    public interface IProgressDialog : IManagedDialog
    {
    }

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

    /// <summary>
    /// Interface for an embedded external user interface implementing Wix# ManagedUI architecture. 
    /// <para>The interface itself is reasonable simple and it basically implements the 
    /// collection/sequence of the runtime UI dialogs with a couple of methods for integrating the ManagedUI
    /// with the MSI.</para>
    /// </summary>
    public interface IManagedUI
    {
        /// <summary>
        /// Gets or sets the id of the 'installdir' (destination folder) directory. It is the directory, 
        /// which is bound to the input UI elements of the Browse dialog (e.g. WiX BrowseDlg, Wix# InstallDirDialog).
        /// </summary>
        /// <value>
        /// The install dir identifier.
        /// </value>
        string InstallDirId { get; set; }
        /// <summary>
        /// Sequence of the dialogs to be displayed during the installation of the product.
        /// </summary>
        ManagedDialogs InstallDialogs { get; }
        /// <summary>
        /// Sequence of the dialogs to be displayed during the customization of the installed product.
        /// </summary>
        ManagedDialogs ModifyDialogs { get; }

        /// <summary>
        /// This method is called (indirectly) by Wix# compiler just befor building the MSI. It allows embedding UI specific resources (e.g. license file, properties)
        /// into the MSI.
        /// </summary>
        /// <param name="project">The project.</param>
        void BeforeBuild(ManagedProject project);
    }


    /// <summary>
    /// Customized version of 'List&lt;Type&gt;', containing Fluent extension methods 
    /// </summary>
    public class ManagedDialogs : List<Type>
    {
        /// <summary>
        /// Adds an typeof(T) object to the end of the collections.
        /// </summary>
        /// <typeparam name="T">Type implementing ManagedUI dialog</typeparam>
        /// <returns></returns>
        public ManagedDialogs Add<T>() where T : IManagedDialog
        {
            base.Add(typeof(T));
            return this;
        }
        /// <summary>
        /// Adds an Type object to the end of the collections.
        /// </summary>
        /// <param name="type">Type implementing ManagedUI dialog.</param>
        /// <returns></returns>
        public new ManagedDialogs Add(Type type)
        {
            base.Add(type);
            return this;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <returns></returns>
        public new ManagedDialogs Clear()
        {
            base.Clear();
            return this;
        }
    }
}
