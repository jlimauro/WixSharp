using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class FeaturesDialog : ManagedForm
    {

        /*https://msdn.microsoft.com/en-us/library/aa367536(v=vs.85).aspx
         * ADDLOCAL - list of features to install 
         * REMOVE - list of features to uninstall 
         * ADDDEFAULT - list of features to set to their default state 
         * REINSTALL - list of features to repair*/


        public FeaturesDialog()
        {
            //Debugger.Launch();
            InitializeComponent();
            ReadOnlyTreeNode.Behavior.AttachTo(featuresTree);
        }

        void FeaturesDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");

            BuildFeaturesHierarchy();
        }

        void BuildFeaturesHierarchy()
        {
            //Cannot use MsiRuntime.Session.Features. 
            //This WiX feature is just not implemented yet. All members except Name throw InvalidHandeException 
            //Thus instead just collect the names and query database for the rest of the properties.
            string[] names = MsiRuntime.Session.Features.Select(x => x.Name).ToArray();

            var features = names.Select(name => new FeatureItem(MsiRuntime.Session, name));

            //build the hierarchy tree
            var rootItems = features.Where(x => x.ParentName == null);

            var itemsToProcess = new Queue<FeatureItem>(rootItems); //features to find the children for

            while (itemsToProcess.Any())
            {
                var item = itemsToProcess.Dequeue();

                //create the view of the feature
                var view = new ReadOnlyTreeNode
                        {
                            Text = item.Title,
                            Tag = item, //link view to model
                            IsReadOnly = item.DisallowAbsent,
                            Checked = item.RequestState != InstallState.Absent
                        };

                item.View = view;

                if (item.Parent != null)
                {
                    (item.Parent.View as TreeNode).Nodes.Add(view); //link child view to parent view
                }

                //find all children
                features.Where(x => x.ParentName == item.Name)
                        .ForEach(c =>
                                 {
                                     c.Parent = item; //link child model to parent model
                                     itemsToProcess.Enqueue(c); //schedule for further processing
                                 });
            }
            
            rootItems.Select(x => x.View)
                     .Cast<TreeNode>()
                     .ForEach(node=>featuresTree.Nodes.Add(node));
        }

        void back_Click(object sender, System.EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, System.EventArgs e)
        {
            var itemsToInstall = featuresTree.AllNodes()
                                             .Where(x => x.Checked)
                                             .Select(x => ((FeatureItem)x.Tag).Name)
                                             .ToArray();

            var itemsToRemove = featuresTree.AllNodes()
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