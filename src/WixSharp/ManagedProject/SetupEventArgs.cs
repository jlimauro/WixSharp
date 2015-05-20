using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace WixSharp
{
    /// <summary>
    ///
    /// </summary>
    public class SetupEventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public enum SetupMode
        {
            /// <summary>
            /// The installing mode
            /// </summary>
            Installing,

            /// <summary>
            /// The repairing mode
            /// </summary>
            Repairing,

            /// <summary>
            /// The uninstalling mode
            /// </summary>
            Uninstalling,

            /// <summary>
            /// The unknown mode
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public Session Session { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public ActionResult Result { get; set; }

        /// <summary>
        /// Gets a value indicating whether Authored UI and wizard dialog boxes suppressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if UI is suppressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsUISupressed { get { return UILevel <= 4; } }

        /// <summary>
        /// Gets the UIlevel.
        /// <para>UILevel > 4 lead to displaying modal dialogs. See https://msdn.microsoft.com/en-us/library/aa369487(v=vs.85).aspx. </para>
        /// </summary>
        /// <value>
        /// The UI level.
        /// </value>
        public int UILevel { get { return Data["UILevel"].ToInt(-1); } }

        /// <summary>
        /// Gets a value indicating whether the event handler is executed from the elevated context.
        /// </summary>
        /// <value>
        /// <c>true</c> if the execution context is elevated; otherwise, <c>false</c>.
        /// </value>
        public bool IsElevated { get { return WindowsIdentity.GetCurrent().IsAdmin(); } }

        /// <summary>
        /// Gets a value indicating whether the product is installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the product is installed; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalled { get { return Data["Installed"].IsNotEmpty(); } }

        /// <summary>
        /// Gets a value indicating whether the product is being installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalling { get { return !IsInstalled && Data["REMOVE"] != "ALL"; } }

        /// <summary>
        /// Gets a value indicating whether the product is being repaired.
        /// </summary>
        /// <value>
        /// <c>true</c> if repairing; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepairing { get { return IsInstalled && Data["REMOVE"] != "ALL"; } }

        /// <summary>
        /// Gets a value indicating whether the product is being uninstalled.
        /// </summary>
        /// <value>
        /// <c>true</c> if uninstalling; otherwise, <c>false</c>.
        /// </value>
        public bool IsUninstalling { get { return Data["REMOVE"] == "ALL"; } }

        /// <summary>
        /// Gets the msi file location.
        /// </summary>
        /// <value>
        /// The msi file.
        /// </value>
        public string MsiFile { get { return Data["MsiFile"]; } }

        /// <summary>
        /// Gets the setup mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public SetupMode Mode
        {
            get
            {
                if (IsInstalling) return SetupMode.Installing;
                if (IsRepairing) return SetupMode.Repairing;
                if (IsUninstalling) return SetupMode.Uninstalling;
                return SetupMode.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the install directory.
        /// </summary>
        /// <value>
        /// The install dir.
        /// </value>
        public string InstallDir
        {
            get { return Session.Property("INSTALLDIR"); }
            set { Session["INSTALLDIR"] = value; }
        }

        ///// <summary>
        ///// Gets the msi window.
        ///// </summary>
        ///// <value>
        ///// The msi window.
        ///// </value>
        //public IntPtr MsiWindow { get { return Data["MsiWindow"].ToIntPtr(); } }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public AppData Data { get; set; }

        //public ResourcesData UIText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupEventArgs"/> class.
        /// </summary>
        public SetupEventArgs()
        {
            Data = new AppData();
            //UIText = new ResourcesData();
        }

        /// <summary>
        /// Saves the user data.
        /// </summary>
        internal void SaveData()
        {
            this.Session["WIXSHARP_RUNTIME_DATA"] = Data.ToString();
        }

        /// <summary>
        ///Class that encapsulated parsing of the CustomActionData content
        /// </summary>
        public class AppData : Dictionary<string, string>
        {
            /// <summary>
            /// Initializes from string.
            /// </summary>
            /// <param name="data">The data.</param>
            public AppData InitFrom(string data)
            {
                this.Clear();
                foreach (var item in data.ToDictionary(itemDelimiter: '\n'))
                    this.Add(item.Key, item.Value.Replace("{$NL}", "\n"));
                return this;
            }

            /// <summary>
            /// Initializes from dictionary.
            /// </summary>
            /// <param name="data">The data.</param>
            public AppData InitFrom(Dictionary<string, string> data)
            {
                this.Clear();
                foreach (var item in data)
                    this.Add(item.Key, item.Value);
                return this;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Join("\n", this.Select(x => x.Key + "=" + x.Value.Replace("\n", "{$NL}")).ToArray());
            }

            public string this[string key]
            {
                get
                {
                    return base.ContainsKey(key) ? base[key] : null;
                }
                set
                {
                    base[key] = value;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                "\nInstallDir=" + InstallDir +
                "\nMsiFile=" + MsiFile +
                "\nUILevel=" + UILevel +
                "\nMode=" + Mode +
                "\nIsElevated=" + IsElevated +
                "\nIsInstalled=" + IsInstalled +
                "\nIsInstalling=" + IsInstalling +
                "\nIsUninstalling=" + IsUninstalling +
                "\nIsReparing=" + IsRepairing;
        }
    }
}
