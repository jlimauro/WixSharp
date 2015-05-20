using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;

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
            }
            catch { }
        }
    }

    public class ResourcesData : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes from WiX localization data (*.wxl).
        /// </summary>
        /// <param name="wxlFile">The WXL file bytes.</param>
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