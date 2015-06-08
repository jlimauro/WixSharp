using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Diagnostics;

[assembly: BootstrapperApplication(typeof(WixSharp.Bootstrapper.SilentManagedBA))]

namespace WixSharp.Bootstrapper
{
    public class SilentBootstrapperApplication : ManagedBootstrapperApplication
    {
        public SilentBootstrapperApplication(string primaryPackageId)
            : base(typeof(SilentManagedBA).Assembly.Location)
        {
            PrimaryPackageId = primaryPackageId;
        }

        public SilentBootstrapperApplication()
            : base(typeof(SilentManagedBA).Assembly.Location)
        {

        }

        public override void AutoGenerateSources(string outDir)
        {
            if (PrimaryPackageId != null)
            {
                string newDef = SilentManagedBA.PrimaryPackageIdVariableName + "=" + PrimaryPackageId;
                if (!StringVariablesDefinition.Contains(newDef))
                    StringVariablesDefinition += ";" + newDef;
            }
            base.AutoGenerateSources(outDir);
        }
    }

    public class SilentManagedBA : BootstrapperApplication
    {
        AutoResetEvent done = new AutoResetEvent(false);

        static public string PrimaryPackageIdVariableName = "_WixSharp.Bootstrapper.SilentManagedBA.PrimaryPackageId";
        string PrimaryPackageId
        {
            get
            {
                try
                {
                    return this.Engine.StringVariables[PrimaryPackageIdVariableName];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Entry point that is called when the bootstrapper application is ready to run.
        /// </summary>
        protected override void Run()
        {
            if (PrimaryPackageId == null)
            {
                MessageBox.Show(PrimaryPackageIdVariableName + " variable is not set", "Wix#");
            }
            else
            {
                ApplyComplete += OnApplyComplete;
                DetectPackageComplete += OnDetectPackageComplete;
                PlanComplete += OnPlanComplete;
                Engine.Detect();
                done.WaitOne();
            }
            Engine.Quit(0);
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
        /// If the planning was successful, it instructs the Bootstrapper Engine to 
        /// install the packages.
        /// </summary>
        void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
                this.Engine.Apply(System.IntPtr.Zero);
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
        /// Checks the PackageId and sets the installation scenario. The PackageId is the ID
        /// specified in one of the package elements (msipackage, exepackage, msppackage,
        /// msupackage) in the WiX bundle.
        /// </summary>
        void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == PrimaryPackageId)
            {
                if (e.State == PackageState.Absent)
                {
                    this.Engine.Plan(LaunchAction.Install);
                }
                else if (e.State == PackageState.Present)
                {
                    this.Engine.Plan(LaunchAction.Uninstall);
                }
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
        /// This is called after a bundle installation has completed. Make sure we updated the view.
        /// </summary>
        void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            done.Set();
        }
    }
}