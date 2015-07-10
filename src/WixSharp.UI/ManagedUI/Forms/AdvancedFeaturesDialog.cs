using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class AdvancedFeaturesDialog : FeaturesDialog
    {
        //At this stage it is a full equivalent of FeaturesDialog
        //Though in the future it can be extended to match the default MSI FeaturesDialog
        //(context menu and icon instead of checkbox)
    }

}