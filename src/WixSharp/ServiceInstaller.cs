
using System.Collections.Generic;
using System.Xml.Linq;
namespace WixSharp
{
    public partial class ServiceInstaller : WixEntity
    {
        public ServiceInstaller() { }
        public ServiceInstaller(string name) 
        {
            this.Name = name;
        }
        public SvcErrorControl ErrorControl = SvcErrorControl.normal;
        public SvcStartType StartType = SvcStartType.auto;
        public string DisplayName = null;
        public string Description = null;
        public SvcType Type = SvcType.ownProcess;
        public SvcEvent StartOn = SvcEvent.Install;
        public SvcEvent StopOn = SvcEvent.InstallUninstall_Wait;
        public SvcEvent RemoveOn = SvcEvent.Uninstall;

        public object[] ToXml()
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
            
            if(StopOn != null)
                result.Add(SvcEventToXml("Stop", StopOn));

            if (RemoveOn != null)
                result.Add(SvcEventToXml("Remove", RemoveOn));

            return result.ToArray();
        }

        XElement SvcEventToXml(string controlType, SvcEvent value)
        {
            return new XElement("ServiceControl",
                       new XAttribute("Id", controlType + this.Name),
                       new XAttribute("Name", this.Name),
                       new XAttribute(controlType, value.Type),
                       new XAttribute("Wait", value.Wait.ToYesNo()));
        }


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

     public class SvcEvent
    {
        public SvcEventType Type;
        public bool Wait;
        
        static public SvcEvent Install = new SvcEvent{Type=SvcEventType.install, Wait = false};
        static public SvcEvent Install_Wait = new SvcEvent{Type=SvcEventType.install, Wait = true};
        static public SvcEvent Uninstall = new SvcEvent{Type=SvcEventType.uninstall, Wait = false};
        static public SvcEvent Uninstall_Wait = new SvcEvent{Type=SvcEventType.uninstall, Wait = true};
        static public SvcEvent InstallUninstall = new SvcEvent{Type=SvcEventType.both, Wait = false};
        static public SvcEvent InstallUninstall_Wait = new SvcEvent{Type=SvcEventType.both, Wait = true};
    }
}