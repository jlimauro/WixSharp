using Microsoft.Deployment.WindowsInstaller;
using System;
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
            /// The reparing mode
            /// </summary>
            Reparing,
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
        /// Gets the UIlevel.
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
        /// Gets a value indicating whether the setup is executed is in the maintenance mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this mode is a maintenance mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsMaintenance { get { return Data["IsMaintenance"] == true.ToString(); } }

        /// <summary>
        /// Gets a value indicating whether the product is being installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalling { get { return !IsInstalled && !IsMaintenance && Data["REMOVE"] != "ALL"; } }

        /// <summary>
        /// Gets a value indicating whether the product is being repaired.
        /// </summary>
        /// <value>
        /// <c>true</c> if repairing; otherwise, <c>false</c>.
        /// </value>
        public bool IsRepairing { get { return IsInstalled && IsMaintenance && Data["REMOVE"] != "ALL"; } }

        /// <summary>
        /// Gets a value indicating whether the product is being uninstalled.
        /// </summary>
        /// <value>
        /// <c>true</c> if uninstalling; otherwise, <c>false</c>.
        /// </value>
        public bool IsUninstalling { get { return Data["REMOVE"] == "ALL"; } }

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
                if (IsRepairing) return SetupMode.Reparing;
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

        /// <summary>
        /// Gets the msi window.
        /// </summary>
        /// <value>
        /// The msi window.
        /// </value>
        public IntPtr MsiWindow { get { return Data["MsiWindow"].ToIntPtr(); } }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public AppData Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupEventArgs"/> class.
        /// </summary>
        public SetupEventArgs()
        {
            Data = new AppData();
        }

        /// <summary>
        ///Class that encapsulated parsing of the CustomActionData content
        /// </summary>
        public class AppData : Dictionary<string, string>
        {
            /// <summary>
            /// Initializes from.
            /// </summary>
            /// <param name="data">The data.</param>
            public void InitFrom(string data)
            {
                this.Clear();
                foreach (var item in data.ToDictionary(itemDelimiter: '\n'))
                    this.Add(item.Key, item.Value);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Join("\n", this.Select(x => x.Key + "=" + x.Value).ToArray());
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
                "\nUILevel=" + UILevel +
                "\nMode=" + Mode +
                "\nMsiWindow=" + MsiWindow +
                "\nIsElevated=" + IsElevated +
                "\nIsInstalled=" + IsInstalled +
                "\nIsMaintenance=" + IsMaintenance +
                "\nIsInstalling=" + IsInstalling +
                "\nIsUninstalling=" + IsUninstalling +
                "\nIsReparing=" + IsRepairing;
        }
    }
}
