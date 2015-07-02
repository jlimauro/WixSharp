using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using sys = System.Windows.Forms;

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
            }
            catch { }
        }

        public string Localize(string text)
        {
            return ResolveProperties(ResolveUIText(text));
        }

        string ResolveUIText(string text)
        {
            if (UIText.ContainsKey(text))
                return UIText[text];
            else
                return text;
        }

        string ResolveProperties(string text)
        {
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
    }

    internal static class UIExtensions
    {
        public static T LocalizeFrom<T>(this T control, MsiRuntime runtime) where T : sys.Control
        {
            var controls = new Queue<T>(new[] { control });

            while (controls.Any())
            {
                var item = controls.Dequeue();

                item.Text = item.Text.LocalizeFrom(runtime.UIText);

                item.Controls
                    .OfType<T>()
                    .ForEach(x => controls.Enqueue(x));
            }
            return control;
        }

        public static string LocalizeFrom(this string text, Dictionary<string, string> resources)
        {
            if (text.IsEmpty()) return text;

            var result = new StringBuilder(text.Length);

            var regex = new Regex(@"\[.+?\]");

            int lastEnd = 0;
            foreach (Match match in regex.Matches(text))
            {
                result.Append(text.Substring(lastEnd, match.Index - lastEnd));
                lastEnd = match.Index + match.Length;

                string key = match.Value.Trim('[', ']');

                if (resources.ContainsKey(key))
                    result.Append(resources[key]);
                else
                    result.Append(match.Value);
            }
            return result.ToString();
        }

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