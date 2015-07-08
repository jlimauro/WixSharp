using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class FeaturesDialog : ManagedForm
    {
        public FeaturesDialog()
        {
            //Debugger.Launch();
            InitializeComponent();
        }

        void FeaturesDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            
            //Cannot use MsiRuntime.Session.Features. 
            //This WiX feature is just not implemented yet. All members except Name throw InvalidHandeException 

            foreach (var feature in MsiRuntime.Session.Features)
            {
                var item = new FeatureItem(MsiRuntime.Session, feature.Name);
                featuresTree.Nodes.Add(new TreeNode(item.Title)
                    {
                        Tag = item,
                        Checked = item.RequestState != InstallState.Absent 
                    });
            }

            //if (MsiRuntime.Session.IsInstalling())
            //    MsiRuntime.Session["ADDLOCAL"] = "Binaries,Documentation";
            //else
            //    MsiRuntime.Session["REMOVE"] = "Documentation";
        }

        /// <summary>
        /// Equivalent of FeatureInfo which is read-only and doesn't work anyway (at least in WiX v3.9)
        /// </summary>
        internal class FeatureItem
        {
            public string Name;
            public string Parent;
            public string Title;
            public string Description;

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
                    Parent = (string)row["Feature_Parent"];
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

        void back_Click(object sender, System.EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, System.EventArgs e)
        {
            var itemsToInstall = featuresTree.Nodes.Cast<TreeNode>()
                                             .Where(x => x.Checked)
                                             .Select(x => ((FeatureItem)x.Tag).Name)
                                             .ToArray();

            var itemsToRemove = featuresTree.Nodes.Cast<TreeNode>()
                                            .Where(x => !x.Checked)
                                            .Select(x => ((FeatureItem)x.Tag).Name)
                                            .ToArray();
            if (itemsToRemove.Any())
                MsiRuntime.Session["REMOVE"] = string.Join(",", itemsToRemove);

            if (itemsToInstall.Any())
                MsiRuntime.Session["ADDLOCAL"] = string.Join(",", itemsToInstall);

            Shell.GoNext();
        }

        void cancel_Click(object sender, System.EventArgs e)
        {
            Shell.Cancel();
        }
    }
}