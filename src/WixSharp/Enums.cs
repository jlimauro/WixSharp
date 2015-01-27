#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted, 
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

namespace WixSharp
{
    /// <summary>
    /// Specifies predefined values for <see cref="Project.UI"/>, 
    /// which control type of User Interface used to interact with user during the installation.
    /// </summary>
    public enum WUI
    {
        /// <summary>
        /// WixUI_ProgressOnly is "no-UI" dialog set which includes only progress bar.
        /// </summary>
        WixUI_ProgressOnly,
        /// <summary>
        /// WixUI_Minimal is the simplest of the built-in WixUI dialog sets.
        /// </summary>
        WixUI_Minimal,
        /// <summary>
        /// WixUI_InstallDir does not allow the user to choose what features to install, but it adds a dialog to 
        /// let the user choose a directory where the product will be installed.
        /// </summary>
        WixUI_InstallDir,
        /// <summary>
        /// WixUI_Common is defines "common" built-in dialog set. It is used to define additional 
        /// custom dialogs.
        /// </summary>
        WixUI_Common,
        /// <summary>
        /// WixUI_FeatureTree built-in dialog set. 
        /// <para>WixUI_FeatureTree is a simpler version of WixUI_Mondo that omits the setup type dialog.</para>
        /// </summary>
        WixUI_FeatureTree,
        /// <summary>
        /// WixUI_Mondo includes the full set of dialogs (hence "Mondo").
        /// </summary>
        WixUI_Mondo,
        /// <summary>
        /// WixUI_Advanced provides the option of a one-click install like WixUI_Minimal, but it also allows directory and feature 
        /// selection like other dialog sets if the user chooses to configure advanced options.
        /// </summary>
        WixUI_Advanced
    }
    /// <summary>
    /// Specifies predefined values for <see cref="Action.Return"/>, 
    /// which controls invoking type of <c>Custom Actions</c>.
    /// </summary>
    public enum Return
    {
        /// <summary>
        /// Indicates that the custom action will run asynchronously but the installer will wait for the return code at sequence end. 
        /// </summary>
        asyncWait,
        /// <summary>
        /// Indicates that the custom action will run asynchronously and execution may continue after the installer terminates. 
        /// </summary>
        asyncNoWait,
        /// <summary>
        /// Indicates that the custom action will run synchronously and the return code will be checked for success.
        /// </summary>
        check,
        /// <summary>
        /// Indicates that the custom action will run synchronously and the return code will not be checked. 
        /// </summary>
        ignore
    }

    //good read: http://stackoverflow.com/questions/5564619/what-is-the-purpose-of-administrative-installation-initiated-using-msiexec-a

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Sequence" />,
    /// which controls which MSI sequence contains corresponding <c>Custom Action</c>.
    /// </summary>
    public enum Sequence
    {
        /// <summary>
        /// <c>Custom Action</c> belongs to <c>InstallExecuteSequence</c>.
        /// </summary>
        InstallExecuteSequence,
        /// <summary>
        /// <c>Custom Action</c> belongs to <c>InstallUISequence</c>.
        /// </summary>
        InstallUISequence,
        /// <summary>
        /// The AdminExecuteSequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed.
        /// </summary>
        AdminExecuteSequence,
        /// <summary>
        /// The AdminUISequence table lists actions that the installer calls in sequence when the top-level ADMIN action is executed and the internal user interface level is set to full UI or reduced UI. The installer skips the actions in this table if the user interface level is set to basic UI or no UI.
        /// </summary>
        AdminUISequence,
        /// <summary>
        /// <c>Custom Action</c> does not belong to any sequence. Use this value when you need <c>Custom Action</c>
        /// to be invoked not from the installation sequence but from another <c>Custonm Action</c>.
        /// </summary>
        NotInSequence
    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Execute"/> attribute, 
    /// which controls at what stage of installtion script <c>Custom Action</c> will be executed.
    /// </summary>
    public enum Execute
    {
        /// <summary>
        /// Indicates that the custom action will run after successful completion of the installation script (at the end of the installation).
        /// </summary>
        commit,
        /// <summary>
        /// Indicates that the custom action runs in-script (possibly with elevated privileges). 
        /// </summary>
        deferred,
        /// <summary>
        /// Indicates that the custom action will only run in the first sequence that runs it. 
        /// </summary>
        firstSequence,

        /// <summary>
        /// Indicates that the custom action will run during normal processing time with user privileges. This is the default. 
        /// </summary>
        immediate,
        /// <summary>
        /// Indicates that the custom action will only run in the first sequence that runs it in the same process. 
        /// </summary>
        oncePerProcess,
        /// <summary>
        /// Indicates that a custom action will run in the rollback sequence when a failure occurs during installation, usually to undo changes made by a deferred custom action. 
        /// </summary>
        rollback,
        /// <summary>
        /// Indicates that a custom action should be run a second time if it was previously run in an earlier sequence.
        /// </summary>
        secondSequence

    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.Step"/>, 
    /// which controls order of <c>Custom Action</c> to be executed.
    /// <para><c>Before</c> or <c>After</c> switch for <c>Custom Action</c> is controlled by <see cref="When"/>.</para>
    /// </summary>
    public enum Step
    {
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallInitialize</c> action.
        /// </summary>
        InstallInitialize,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallFinalize</c> action.
        /// </summary>
        InstallFinalize,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallFiles</c> action.
        /// </summary>
        InstallFiles,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>RemoveFiles</c> action.
        /// </summary>
        RemoveFiles,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallExecute</c> action.
        /// </summary>
        InstallExecute,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallExecuteAgain</c> action.
        /// </summary>
        InstallExecuteAgain,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action declared in <see cref="Project.Actions"/>.
        /// </summary>
        PreviousAction,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>LaunchConditions</c> action.
        /// </summary>
        LaunchConditions,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after MSI built-in <c>InstallValidate</c> action.
        /// </summary>
        InstallValidate,
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action item in <see cref="Project.Actions"/>.
        /// If <c>Custom Action</c> is the first item in item in <see cref="Project.Actions"/> it will be executed before/after
        /// MSI built-in <c>InstallFinalize</c> action.
        /// </summary>
        PreviousActionOrInstallFinalize, //if it is a the first usage of CA the same as "InstallFinalize" if not "PreviousAction"
        /// <summary>
        /// <c>Custom Action</c> is to be executed before/after the previous action item in <see cref="Project.Actions"/>.
        /// If <c>Custom Action</c> is the first item in item in <see cref="Project.Actions"/> it will be executed before/after
        /// MSI built-in <c>InstallInitialize</c> action.
        /// </summary>
        PreviousActionOrInstallInitialize, //if it is a the first usage of CA the same as "InstallInitialize" if not "PreviousAction"

    }

    /// <summary>
    /// Specifies predefined values for <see cref="Action.When"/>, 
    /// which controls sequence of the <c>Custom Action</c> with respect its related, order controlling 
    /// <c>Action</c>.
    /// <para>Order controlling action is defined by <see cref="Step"/>.</para>
    /// </summary>
    public enum When
    {
        /// <summary>
        /// Execute after order controlling action. 
        /// </summary>
        After,
        /// <summary>
        /// Execute before order controlling action. 
        /// </summary>
        Before
    }

    /// <summary>
    /// Sets the default script language (<see cref="IISVirtualDir.DefaultScript"/>) for the Web site.
    /// </summary>
    public enum DefaultScript
    {
        /// <summary>
        /// 
        /// </summary>
        VBScript,
        /// <summary>
        /// 
        /// </summary>
        JScript
    }

    /// <summary>
    /// Sets the (<see cref="T:IISVirtualDir.Certificate.StoreLocation"/>) for the Web site certificate. 
    /// </summary>
    public enum StoreLocation
    {
        /// <summary>
        /// 
        /// </summary>
        currentUser,
        /// <summary>
        /// 
        /// </summary>
        localMachine
    }
    /// <summary>
    /// Sets the (<see cref="T:IISVirtualDir.Certificate.StoreName"/>) for the Web site certificate.
    /// </summary>
    public enum StoreName
    {
        /// <summary>
        /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates 
        /// in these stores are normally supplied with the operating system or by the user's network administrator. 
        /// </summary>
        ca,
        /// <summary>
        /// Use the "personal" value instead. 
        /// </summary>
        my,
        /// <summary>
        /// Contains personal certificates. These certificates will usually have an associated private key. This store is often referred to as the "MY" certificate store. 
        /// </summary>
        personal,
        /// <summary>
        /// 
        /// </summary>
        request,
        /// <summary>
        /// Contains the certificates of certificate authorities that the user trusts to issue certificates to others. Certificates in these stores are normally supplied with the operating system or by the user's network administrator. Certificates in this store are typically self-signed. 
        /// </summary>
        root,
        /// <summary>
        /// Contains the certificates of those that the user normally sends enveloped messages to or receives signed messages from. See MSDN documentation for more information. 
        /// </summary>
        otherPeople
    }
    /// <summary>
    /// Values of the application isolation level of <see cref="IISVirtualDir.Isolation"/> for pre-IIS 6 applications
    /// </summary>
    public enum Isolation
    {
        /// <summary>
        /// Means the application executes within the IIS process. 
        /// </summary>
        low,
        /// <summary>
        /// Executes pooled in a separate process. 
        /// </summary>
        medium,
        /// <summary>
        /// Means execution alone in a separate process. 
        /// </summary>
        high
    }

    /// <summary>
    /// Determines what service action should be taken on an error. 
    /// </summary>
    public enum SvcErrorControl
    {
        /// <summary>
        ///Logs the error and continues with the startup operation.
        /// </summary>
        ignore,
        /// <summary>
        ///Logs the error, displays a message box and continues the startup operation.
        /// </summary>
        normal,
        /// <summary>
        /// Logs the error if it is possible and the system is restarted with the last configuration known to be good. If the last-known-good configuration is being started, the startup operation fails.
        /// </summary>
        critical
    }

    /// <summary>
    /// Determines when the service should be started. The Windows Installer does not support boot or system. 
    /// </summary>
    public enum SvcStartType
    {
        /// <summary>
        /// The service will start during startup of the system.
        /// </summary>
        auto,
        /// <summary>
        ///The service will start when the service control manager calls the StartService function.        
        /// </summary>
        demand,
        /// <summary>
        /// The service can no longer be started.
        /// </summary>
        disabled,
        /// <summary>
        /// The service is a device driver that will be started by the operating system boot loader. This value is not currently supported by the Windows Installer.
        /// </summary>
        boot,
        /// <summary>
        /// The service is a device driver that will be started by the IoInitSystem function. This value is not currently supported by the Windows Installer.
        /// </summary>
        system
    }


    /// <summary>
    /// The Windows Installer does not currently support kernelDriver or systemDriver. This attribute's value must be one of the following:
    /// </summary>
    public enum SvcType
    {
        /// <summary>
        /// A Win32 service that runs its own process.
        /// </summary>
        ownProcess,
        /// <summary>
        /// A Win32 service that shares a process.
        /// </summary>
        shareProcess,
        /// <summary>
        /// A kernel driver service. This value is not currently supported by the Windows Installer.
        /// </summary>
        kernelDriver,
        /// <summary>
        /// A file system driver service. This value is not currently supported by the Windows Installer.
        /// </summary>
        systemDriver
    }

    /// <summary>
    /// Specifies whether an action occur on install, uninstall or both.
    /// </summary>
    public enum SvcEventType
    {
        /// <summary>
        /// Specifies that occur on install.
        /// </summary>
        install,
        /// <summary>
        /// Specifies that occur on uninstall.
        /// </summary>
        uninstall,
        /// <summary>
        /// Specifies that occur on install and uninstall.
        /// </summary>
        both
    }
}
