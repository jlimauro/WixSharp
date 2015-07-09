using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Linq;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// Equivalent of FeatureInfo which is read-only and doesn't work anyway (at least in WiX v3.9)
    /// </summary>
    internal class FeatureItem
    {
        public string Name;
        public string ParentName;
        public string Title;
        public string Description;
        
        public object View;
        public FeatureItem Parent;

        public InstallState RequestState;
        public InstallState CurrentState;
        public FeatureAttributes Attributes;

        public bool DisallowAbsent
        {
            get { return FeatureAttributes.UIDisallowAbsent.PresentIn(Attributes); }
        }

        public FeatureItem()
        {
        }

        public FeatureItem(Session session, string name)
        {
            var data = session.OpenView("select * from Feature where Feature = '" + name + "'", "Feature, Feature_Parent, Title, Description, Display, Level, Directory_, Attributes");

            Dictionary<string, object> row = data.FirstOrDefault();

            if (row != null)
            {
                Name = name;
                ParentName = (string)row["Feature_Parent"];
                Title = (string)row["Title"].ToString();
                Description = (string)row["Description"];

                var defaultState = (InstallState)row["Level"];

                CurrentState = DetectFeatureState(session, name);
                RequestState = session.IsInstalled() ? CurrentState : defaultState;

                Attributes = (FeatureAttributes)row["Attributes"];
            }
        }

        public override string ToString()
        {
            return Title;
        }

        static InstallState DetectFeatureState(Session session, string name)
        {
            var productCode = session["ProductCode"];

            var installedPackage = new Microsoft.Deployment.WindowsInstaller.ProductInstallation(productCode);
            if (installedPackage.IsInstalled)
                return installedPackage.Features
                .First(x => x.FeatureName == name)
                .State;
            else
                return InstallState.Absent;
        }
    }
}