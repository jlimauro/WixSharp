using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    ///  Defines service installer for the file being installed. It encapsulates functionality provided
    ///  by <c>ServiceInstall</c> and <c>ServiceConfig</c> WiX elements.
    /// </summary>
    /// <example>The following sample demonstrates how to install service:
    /// <code>
    /// File service;
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             service = new File(@"..\SimpleService\MyApp.exe")));
    ///
    /// service.ServiceInstaller = new ServiceInstaller
    ///                            {
    ///                                Name = "WixSharp.TestSvc",
    ///                                StartOn = SvcEvent.Install,
    ///                                StopOn = SvcEvent.InstallUninstall_Wait,
    ///                                RemoveOn = SvcEvent.Uninstall_Wait,
    ///                            };
    ///  ...
    ///
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class ServiceInstaller : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller"/> class.
        /// </summary>
        public ServiceInstaller()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceInstaller(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The error control associated with the service startup. The default value is <c>SvcErrorControl.normal</c>.
        /// </summary>
        public SvcErrorControl ErrorControl = SvcErrorControl.normal;

        /// <summary>
        /// Defines the way service starts. The default value is <c>SvcStartType.auto</c>.
        /// </summary>
        public SvcStartType StartType = SvcStartType.auto;

        /// <summary>
        /// The display name of the service as it is listed in the Control Panel.
        /// If not specified the name of the service will be used instead.
        /// </summary>
        public string DisplayName = null;

        /// <summary>
        /// The description of the service as it is listed in the Control Panel.
        /// </summary>
        public string Description = null;

        /// <summary>
        /// The type of the service (e.g. kernel/system driver, process). The default value is <c>SvcType.ownProcess</c>.
        /// </summary>
        public SvcType Type = SvcType.ownProcess;

        /// <summary>
        /// Associates 'start service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Install</c>. Meaning "start the service when it is installed". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to start the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StartOn = SvcEvent.Install;
        /// <summary>
        /// Associates 'stop service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.InstallUninstall_Wait</c>. Meaning "stop the  
        /// service when it is being installed and uninstalled and wait for the action completion". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to stop the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent StopOn = SvcEvent.InstallUninstall_Wait;
        /// <summary>
        /// Associates 'remove service' action with the specific service installation event.
        /// The default value is <c>SvcEvent.Uninstall</c>. Meaning "remove the service when it is uninstalled". 
        /// <para>
        /// Set this member to <c>null</c> if you don't want to remove the service at any stage of the installation.
        /// </para>
        /// </summary>
        public SvcEvent RemoveOn = SvcEvent.Uninstall;

        internal object[] ToXml()
        {
            var result = new List<object>();

            result.Add(new XElement("ServiceInstall",
                           new XAttribute("Id", Id),
                           new XAttribute("Name", Name),
                           new XAttribute("DisplayName", DisplayName ?? Name),
                           new XAttribute("Description", Description ?? DisplayName ?? Name),
                           new XAttribute("Type", Type),
                           new XAttribute("Start", StartType),
                           new XAttribute("ErrorControl", ErrorControl))
                           .AddAttributes(Attributes));

            if (StartOn != null)
                result.Add(SvcEventToXml("Start", StartOn));

            if (StopOn != null)
                result.Add(SvcEventToXml("Stop", StopOn));

            if (RemoveOn != null)
                result.Add(SvcEventToXml("Remove", RemoveOn));

            return result.ToArray();
        }

        XElement SvcEventToXml(string controlType, SvcEvent value)
        {
            return new XElement("ServiceControl",
                       new XAttribute("Id", controlType + this.Id),
                       new XAttribute("Name", this.Name),
                       new XAttribute(controlType, value.Type),
                       new XAttribute("Wait", value.Wait.ToYesNo()));
        }

        //Raw WiX sample: 
        //<ServiceInstall Id="ABC"
        //                Name="WixServiceInstaller"
        //                DisplayName="WixServiceInstaller"
        //                Type="ownProcess"
        //                Start="auto"
        //                ErrorControl="normal"
        //                Description="WixServiceInstaller"
        //                Account="[SERVICEACCOUNT]"
        //                Password="[SERVICEPASSWORD]" />
        //<ServiceControl Id="StartWixServiceInstaller"
        //                Name="WixServiceInstaller" Start="install" Wait="no" />
        //<ServiceControl Id="StopWixServiceInstaller" Name="WixServiceInstaller"
        //                Stop="both" Wait="yes" Remove="uninstall" />
    }

    /// <summary>
    /// Defines details of the setup event triggering the service actions to be performed during the service installation.
    /// </summary>
    public class SvcEvent
    {
        /// <summary>
        /// Specifies when the service action occur. It can be one of the <see cref="SvcEventType"/> values.
        /// </summary>
        public SvcEventType Type;
        /// <summary>
        /// The flag indicating if after triggering the service action the setup should wait until te action is completed.
        /// </summary>
        public bool Wait;

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Install = new SvcEvent { Type = SvcEventType.install, Wait = false };

        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.install"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Install_Wait = new SvcEvent { Type = SvcEventType.install, Wait = true };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation 
        /// without waiting for the completion.
        /// </summary>
        static public SvcEvent Uninstall = new SvcEvent { Type = SvcEventType.uninstall, Wait = false };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.uninstall"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent Uninstall_Wait = new SvcEvent { Type = SvcEventType.uninstall, Wait = true };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>false</c>. It triggers the action during the service installation without 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent InstallUninstall = new SvcEvent { Type = SvcEventType.both, Wait = false };
        /// <summary>
        /// Predefined instance of <see cref="SvcEvent"/> with <c>Type</c> set to <see cref="SvcEventType.both"/>
        /// and <c>Wait</c> to <c>true</c>. It triggers the action during the service installation with 
        /// waiting for the completion.
        /// </summary>
        static public SvcEvent InstallUninstall_Wait = new SvcEvent { Type = SvcEventType.both, Wait = true };
    }
}