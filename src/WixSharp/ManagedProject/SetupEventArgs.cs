using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;

namespace WixSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class SetupEventArgs
    {
        public enum SetupType
        {
            Installing,
            Reparing,
            Uninstalling,
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


        public bool IsElevated { get { return WindowsIdentity.GetCurrent().IsAdmin(); } }
        public bool IsInstalled { get { return Data["Installed"].IsNotEmpty(); } }
        public bool IsMaintenance { get { return Data["IsMaintenance"] == true.ToString(); } }

        public bool IsInstalling { get { return IsInstalled && !IsMaintenance; } }
        public bool IsReparing { get { return IsInstalled && !IsMaintenance; } }
        public bool IsUninstalling { get { return Data["REMOVE"] == "ALL"; } }

        public SetupType Type
        {
            get
            {
                if(IsInstalling) return SetupType.Installing;
                if(IsReparing) return SetupType.Reparing;
                if(IsUninstalling) return SetupType.Uninstalling;
                return SetupType.Unknown;
            }
        }
        string installDir;

        public string InstallDir
        {
            get { return Session.Property("INSTALLDIR"); }
            set { Session["INSTALLDIR"] = value; }
        }

        // bool isElevated;

        public IntPtr MsiWindow { get { return Data["MsiWindow"].ToIntPtr(); } }

        //see ManagedProject.Init(Session session) for the list of Data embedded properties 
        public AppData Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupEventArgs"/> class.
        /// </summary>
        public SetupEventArgs()
        {
            Data = new AppData();
        }

        /// <summary>
        /// 
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
            return Data.ToString() + 
                "\nINSTALLDIR=" + InstallDir + 
                "\nIsElevated=" + IsElevated +
                "\nIsInstalling=" + IsInstalling +
                "\nIsInstalling=" + IsReparing;
        }
    }


}
