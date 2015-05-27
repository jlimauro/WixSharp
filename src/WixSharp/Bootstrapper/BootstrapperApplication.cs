using System;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    /// <summary>
    /// Container class for common members of the Bootstrapper standard applications.
    /// </summary>
    public abstract class WixStandardBootstrapperApplication : WixEntity
    {
        /// <summary>
        /// The predefined instance of the License-based bootstrapper application.
        /// </summary>
        public static LicenseBootstrapperApplication License = new LicenseBootstrapperApplication();

        /// <summary>
        /// Source file of the RTF license file or URL target of the license link.
        /// </summary>
        public string LicensePath;

        /// <summary>
        /// Source file of the logo graphic.
        /// </summary>
        [Xml]
        public string LogoFile;

        /// <summary>
        /// Source file of the theme localization .wxl file.
        /// </summary>
        [Xml]
        public string LocalizationFile;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public abstract XContainer[] ToXml();
    }


    /// <summary>
    /// Generic License-based WiX bootstrapper application. 
    /// <para>Depending on the value of LicensePath compiler will resolve the application in either <c>WixStandardBootstrapperApplication.RtfLicense</c>
    /// or <c>WixStandardBootstrapperApplication.HyperlinkLicense</c> standard application.</para>
    /// <para>Note: empty LicensePath will suppress displaying the license completely</para>
    /// </summary>
    /// <example>The following is an example of defining a simple bootstrapper displaying the license as an 
    /// embedded HTML file.
    /// <code>
    /// var bootstrapper = new Bundle("My Product",
    ///                         new PackageGroupRef("NetFx40Web"),
    ///                         new MsiPackage(productMsi) { DisplayInternalUI = true });
    ///                         
    /// bootstrapper.AboutUrl = "https://wixsharp.codeplex.com/";
    /// bootstrapper.IconFile = "app_icon.ico";
    /// bootstrapper.Version = new Version("1.0.0.0");
    /// bootstrapper.UpgradeCode = new Guid("6f330b47-2577-43ad-9095-1861bb25889b");
    /// bootstrapper.Application.LogoFile = "logo.png";
    /// bootstrapper.Application.LicensePath = "licence.html"; 
    /// 
    /// bootstrapper.Build();
    /// </code>
    /// </example>
    public class LicenseBootstrapperApplication : WixStandardBootstrapperApplication
    {
        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public override XContainer[] ToXml()
        {
            XNamespace bal = "http://schemas.microsoft.com/wix/BalExtension";

            var root = new XElement("BootstrapperApplicationRef");

            var app = new XElement(bal + "WixStandardBootstrapperApplication");

            app.Add(this.MapToXmlAttributes());

            if (LicensePath.IsNotEmpty() && LicensePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
            {
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.RtfLicense");
                app.SetAttribute("LicenseFile", LicensePath);
            }
            else
            {
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.HyperlinkLicense");

                if (LicensePath.IsEmpty())
                {
                    //cannot use SetAttribute as we want to preserve empty attrs
                    app.Add(new XAttribute("LicenseUrl", ""));
                }
                else
                {
                    if (LicensePath.StartsWith("http")) //online HTML file
                    {
                        app.SetAttribute("LicenseUrl", LicensePath);
                    }
                    else
                    {
                        app.SetAttribute("LicenseUrl", System.IO.Path.GetFileName(LicensePath));
                        root.AddElement("Payload").AddAttributes("SourceFile=" + LicensePath);
                    }
                }
            }

            root.Add(app);

            return new[] { root };
        }
    }
}