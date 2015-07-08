using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using io = System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using sys = System.Windows.Forms;
using System.Drawing.Imaging;

#pragma warning disable 1591

namespace WixSharp
{
    public class MsiRuntime
    {
        public System.Action StartExecute;
        public Session Session;
        public ResourcesData UIText = new ResourcesData();
        public MsiRuntime(Session session)
        {
            this.Session = session;
            try
            {
                var bytes = session.TryReadBinary("WixSharp_UIText");
                UIText.InitFromWxl(bytes);
                ProductName = session.Property("ProductName");
                ProductCode = session.Property("ProductCode");
                ProductVersion = session.Property("ProductVersion");

                //it is important to preserve some product properties for localization as at the end of setup session will no longer be available
                UIText["ProductName"] = ProductName;
                UIText["ProductCode"] = ProductCode;
                UIText["ProductVersion"] = ProductVersion;

                //ensure Wix# strings are added if not already present
                if (!UIText.ContainsKey("ViewLog"))
                    UIText["ViewLog"] = "View Log";
            }
            catch { }
        }

        public Bitmap GetMsiBitmap(string name)
        {
            try
            {
                byte[] data = Session.ReadBinary(name);
                using (Stream s = new MemoryStream(data))
                    return (Bitmap)Bitmap.FromStream(s);
            }
            catch { }
            return null;
        }

        public string Localize(string text)
        {
            if (UIText.ContainsKey(text))
                return UIText[text];

            try
            {
                string result = Session.Property(text);
                if (result.IsEmpty())
                    return text;
                else
                    return result;
            }
            catch { }

            return text;
        }

        public string ProductName;
        public string ProductCode;
        public string ProductVersion;
    }

    public class ResourcesData : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes from WiX localization data (*.wxl).
        /// </summary>
        /// <param name="wxlData">The WXL file bytes.</param>
        /// <returns></returns>
        public void InitFromWxl(byte[] wxlData)
        {
            this.Clear();
            if (wxlData != null && wxlData.Any())
            {
                var data = XDocument.Parse(wxlData.GetString(Encoding.UTF8))
                                    .Descendants()
                                    .Where(x => x.Name.LocalName == "String")
                                    .ToDictionary(x => x.Attribute("Id").Value, x => x.Value);
                foreach (var item in data)
                    this.Add(item.Key, item.Value);
            }
        }

        public new string this[string key]
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

}