using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using System.Text;

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
        /// Gets the msi file location.
        /// </summary>
        /// <value>
        /// The msi file.
        /// </value>
        public string MsiFile { get { return Data["MsiFile"]; } }


        public bool HasBeforeSetupClrDialogs { get { return Session.Property(GetClrDialogsPropertyName(true)).IsNotEmpty(); } }
        public bool HasAfterSetupClrDialogs { get { return Session.Property(GetClrDialogsPropertyName(false)).IsNotEmpty(); } }

        string GetClrDialogsPropertyName(bool isBefore)
        {
            if (IsInstalling) return (isBefore ? "WixSharp_BeforeInstall_Dialogs" : "WixSharp_AfterInstall_Dialogs");
            if (IsRepairing) return (isBefore ? "WixSharp_BeforeRepair_Dialogs" : "WixSharp_AfterRepair_Dialogs");
            if (IsUninstalling) return (isBefore ? "WixSharp_BeforeUninstall_Dialogs" : "WixSharp_AfterUninstall_Dialogs");
            return "unknown";
        }

        public Type[] BeforeSetupClrDialogs
        {
            get
            {
                try
                {
                    string name = GetClrDialogsPropertyName(true);
                    return Session.Property(name)
                                  .Split('\n')
                                  .Select(x => x.Trim())
                                  .Where(x => x.IsNotEmpty())
                                  .Select(x => ManagedProject.GetDialog(x))
                                  .ToArray();
                }
                catch
                {
                    return new Type[0];
                }
            }
        }

        public Type[] AfterInstallClrDialogs
        {
            get
            {
                try
                {
                    string name = GetClrDialogsPropertyName(false);
                    return Session.Property(name)
                                  .Split('\n')
                                  .Select(x => x.Trim())
                                  .Where(x => x.IsNotEmpty())
                                  .Select(x => ManagedProject.GetDialog(x))
                                  .ToArray();
                }
                catch
                {
                    return new Type[0];
                }
            }
        }

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
        public ResourcesData UIText { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupEventArgs"/> class.
        /// </summary>
        public SetupEventArgs()
        {
            Data = new AppData();
            UIText = new ResourcesData();
        }

        /// <summary>
        /// Saves the user data.
        /// </summary>
        internal void SaveData()
        {
            this.Session["WIXSHARP_RUNTIME_DATA"] = Data.ToString();
        }

        public class ResourcesData : AppData
        {
            /// <summary>
            /// Initializes from WiX localization data (*.wxl).
            /// </summary>
            /// <param name="wxlFile">The WXL file.</param>
            /// <returns></returns>
            public ResourcesData InitFromWxl(byte[] wxlData)
            {
                var data = XDocument.Parse(wxlData.GetString(Encoding.UTF8))
                                    .Descendants()
                                    .Where(x => x.Name.LocalName == "String")
                                    .ToDictionary(x => x.Attribute("Id").Value, x => x.Value);
                base.InitFrom(data);
                return this;
            }

            /// <summary>
            /// Initializes from string.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            public ResourcesData InitFrom(string data)
            {
                var map = data.DecodeFromHex().GetString(Encoding.UTF8);
                base.InitFrom(map);
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
                string data = base.ToString();
                return data.GetBytes(Encoding.UTF8).EncodeToHex();
            }
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
