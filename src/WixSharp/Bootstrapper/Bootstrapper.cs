using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    //Useful stuff to have a look at: 
    //http://neilsleightholm.blogspot.com.au/2012/05/wix-burn-tipstricks.html
    //https://wixwpf.codeplex.com/
    public partial class StandardBootstrapper : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public StandardBootstrapper()
        {
            WixExtensions.Add("WiXNetFxExtension");
            WixExtensions.Add("WiXBalFxExtension");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper" /> class.
        /// </summary>
        /// <param name="name">The name of the project. Typically it is the name of the product to be installed.</param>
        /// <param name="items">The project installable items (e.g. directories, files, registry keys, Custom Actions).</param>
        public StandardBootstrapper(string name, params ChainItem[] items)
        {
            WixExtensions.Add("WiXNetFxExtension");
            WixExtensions.Add("WiXBalExtension");
            Name = name;
            Chain.AddRange(items);
        }

        string sourceBaseDir = "";

        /// <summary>
        /// Base directory for the relative paths of the bootstrapper items (e.g. <see cref="MsiPackage"></see>).
        /// </summary>
        public string SourceBaseDir
        {
            get { return sourceBaseDir.ExpandEnvVars(); }
            set { sourceBaseDir = value; }
        }

        string outFileName = "setup";
         
        /// <summary>
        /// Name of the MSI/MSM file (without extension) to be build.
        /// </summary>
        public string OutFileName { get { return outFileName; } set { outFileName = value; } }

        string outDir;

        /// <summary>
        /// The output directory. The directory where all msi and temporary files should be assembled. The <c>CurrentDirectory</c> will be used if <see cref="OutDir"/> is left unassigned.
        /// </summary>
        public string OutDir
        {
            get
            {
                return outDir.IsEmpty() ? Environment.CurrentDirectory : outDir.ExpandEnvVars();
            }
            set
            {
                outDir = value;
            }
        }

        /// <summary>
        /// The disable rollbackSpecifies whether the bundle will attempt to rollback packages executed in the chain.
        /// If "yes" is specified then when a vital package fails to install only that package will rollback and the chain will stop with the error.
        /// The default is "no" which indicates all packages executed during the chain will be rolldback to their previous state when a vital package fails.
        /// </summary>
        [Xml]
        public bool? DisableRollback;

        /// <summary>
        /// Specifies whether the bundle will attempt to create a system restore point when executing the chain. If "yes" is specified then a system restore point will not be created. The default is "no" which indicates a system restore point will be created when the bundle is installed, uninstalled, repaired, modified, etc. If the system restore point cannot be created, the bundle will log the issue and continue.
        /// </summary>
        [Xml]
        public bool? DisableSystemRestore;

        /// <summary>
        /// The legal copyright found in the version resources of final bundle executable.
        /// If this attribute is not provided the copyright will be set to "Copyright (c) [Bundle/@Manufacturer]. All rights reserved.".
        /// </summary>
        [Xml]
        public string Copyright;

        /// <summary>
        /// A URL for more information about the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string AboutUrl;

        /// <summary>
        /// Whether Packages and Payloads not assigned to a container should be added to the default attached container or if they
        /// should be external. The default is yes.
        /// </summary>
        [Xml]
        public bool? Compressed;

        /// <summary>
        /// The condition of the bundle. If the condition is not met, the bundle will refuse to run. Conditions are checked before the
        /// bootstrapper application is loaded (before detect), and thus can only reference built-in variables such as variables which
        /// indicate the version of the OS.
        /// </summary>
        [Xml]
        public string Condition;

        /// <summary>
        /// Determines whether the bundle can be removed via the Programs and Features (also known as Add/Remove Programs). If the value is
        /// "yes" then the "Uninstall" button will not be displayed. The default is "no" which ensures there is an "Uninstall" button to remove
        /// the bundle. If the "DisableModify" attribute is also "yes" or "button" then the bundle will not be displayed in Progams and
        /// Features and another mechanism (such as registering as a related bundle addon) must be used to ensure the bundle can be removed.
        /// </summary>
        [Xml]
        public bool? DisableRemove;

        /// <summary>
        /// Determines whether the bundle can be modified via the Programs and Features (also known as Add/Remove Programs). If the value is
        /// "button" then Programs and Features will show a single "Uninstall/Change" button. If the value is "yes" then Programs and Features
        /// will only show the "Uninstall" button". If the value is "no", the default, then a "Change" button is shown. See the DisableRemove
        /// attribute for information how to not display the bundle in Programs and Features.
        /// </summary>
        [Xml]
        public string DisableModify;

        /// <summary>
        /// A telephone number for help to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string HelpTelephone;

        /// <summary>
        /// A URL to the help for the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string HelpUrl;

        /// <summary>
        /// Path to an icon that will replace the default icon in the final Bundle executable. This icon will also be displayed in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml(Name = "IconSourceFile")]
        public string IconFile;

        /// <summary>
        /// The publisher of the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string Manufacturer;

        /// <summary>
        /// The name of the parent bundle to display in Installed Updates (also known as Add/Remove Programs). This name is used to nest or group bundles that will appear as updates. If the
        /// parent name does not actually exist, a virtual parent is created automatically.
        /// </summary>
        [Xml]
        public string ParentName;

        /// <summary>
        /// Path to a bitmap that will be shown as the bootstrapper application is being loaded. If this attribute is not specified, no splash screen will be displayed.
        /// </summary>
        [Xml(Name = "SplashScreenSourceFile")]
        public string SplashScreenSource;

        /// <summary>
        /// Set this string to uniquely identify this bundle to its own BA, and to related bundles. The value of this string only matters to the BA, and its value has no direct
        /// effect on engine functionality.
        /// </summary>
        [Xml]
        public string Tag;

        /// <summary>
        /// A URL for updates of the bundle to display in Programs and Features (also known as Add/Remove Programs).
        /// </summary>
        [Xml]
        public string UpdateUrl;

        /// <summary>
        /// Unique identifier for a family of bundles. If two bundles have the same UpgradeCode the bundle with the highest version will be installed.
        /// </summary>
        [Xml]
        public Guid UpgradeCode = Guid.NewGuid();

        /// <summary>
        /// The version of the bundle. Newer versions upgrade earlier versions of the bundles with matching UpgradeCodes. If the bundle is registered in Programs and Features then this attribute will be displayed in the Programs and Features user interface.
        /// </summary>
        [Xml]
        public Version Version;

        /// <summary>
        /// The sequence of the packages to be installed
        /// </summary>
        public List<ChainItem> Chain = new List<ChainItem>();

        public WixStandardBootstrapperApplication Application = new LicenseBootstrapperApplication();

        /// <summary>
        /// Collection of XML namespaces (e.g. <c>xmlns:iis="http://schemas.microsoft.com/wix/IIsExtension"</c>) to be declared in the XML (WiX project) root.
        /// </summary>
        public List<string> WixNamespaces = new List<string>();

        /// <summary>
        /// Collection of paths to the WiX extensions.
        /// </summary>
        public List<string> WixExtensions = new List<string>();

        /// <summary>
        /// Installation UI Language. If not specified <c>"en-US"</c> will be used.
        /// </summary>
        public string Language = "en-US";

        public XContainer[] ToXml()
        {
            var result = new List<XContainer>();

            var root = new XElement("Bundle",
                           new XAttribute("Name", Name));

            root.AddAttributes(this.Attributes);
            root.Add(this.MapToXmlAttributes());


            root.Add(Application.ToXml());

            var xChain = root.AddElement("Chain");
            foreach (var item in this.Chain)
                xChain.Add(item.ToXml());

            result.Add(root);
            return result.ToArray();
        }
    }

    /*
      <Bundle Name="My Product"
            Version="1.0.0.0"
            Manufacturer="OSH"
            AboutUrl="https://wixsharp.codeplex.com/"
            IconSourceFile="app_icon.ico"
            UpgradeCode="acaa3540-97e0-44e4-ae7a-28c20d410a60">

        <BootstrapperApplicationRef Id="WixStandardBootstrapperApplication.RtfLicense">
            <bal:WixStandardBootstrapperApplication LicenseFile="readme.txt" LocalizationFile="" LogoFile="app_icon.ico" />
        </BootstrapperApplicationRef>

        <Chain>
            <!-- Install .Net 4 Full -->
            <PackageGroupRef Id="NetFx40Web"/>
            <!--<ExePackage
                Id="Netfx4FullExe"
                Cache="no"
                Compressed="no"
                PerMachine="yes"
                Permanent="yes"
                Vital="yes"
                SourceFile="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bootstrapper\Packages\DotNetFX40\dotNetFx40_Full_x86_x64.exe"
                InstallCommand="/q /norestart /ChainingPackage FullX64Bootstrapper"
                DetectCondition="NETFRAMEWORK35='#1'"
                DownloadUrl="http://go.microsoft.com/fwlink/?LinkId=164193" />-->

            <RollbackBoundary />

            <MsiPackage SourceFile="E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Managed Setup\ManagedSetup.msi" Vital="yes" />
        </Chain>
    </Bundle>
     */
}
