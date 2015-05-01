using System;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    public class SetupEventArgs
    {
        public Session Session { get; set; }

        public ActionResult Result { get; set; }

        public bool IsInstall { get; set; }

        public bool IsUninstall { get; set; }

        public bool IsRepair { get; set; }
    }
}
