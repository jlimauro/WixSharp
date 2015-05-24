using System;
using System.Xml.Linq;

namespace WixSharp.Bootstrapper
{
    public abstract class WixStandardBootstrapperApplication : WixEntity
    {
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

        public abstract XContainer[] ToXml();
    }


    public class LicenseBootstrapperApplication : WixStandardBootstrapperApplication
    {
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