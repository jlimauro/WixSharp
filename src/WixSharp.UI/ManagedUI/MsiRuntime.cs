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
    }

    //public class ClrDialogs
    //{
    //    static Type WelcomeDialog = typeof(WelcomeDialog);
    //    static Type LicenceDialog = typeof(LicenceDialog);
    //    static Type FeaturesDialog = typeof(FeaturesDialog);
    //    static Type InstallDirDialog = typeof(InstallDirDialog);
    //    static Type ExitDialog = typeof(ExitDialog);

    //    static Type RepairStartDialog = typeof(RepairStartDialog);
    //    static Type RepairExitDialog = typeof(RepairExitDialog);

    //    static Type ProgressDialog = typeof(ProgressDialog);
    //}

    internal static class UIExtensions
    {
        public static System.Drawing.Icon GetAssiciatedIcon(this string extension)
        {
            var dummy = Path.GetTempPath() + extension;
            System.IO.File.WriteAllText(dummy, "");
            var result = System.Drawing.Icon.ExtractAssociatedIcon(dummy);
            System.IO.File.Delete(dummy);
            return result;
        }

        public static sys.Control ClearChildren(this sys.Control control)
        {
            foreach (sys.Control item in control.Controls)
                item.Dispose();

            control.Controls.Clear();
            return control;
        }

        internal static string UserOrDefaultContentOf(string extenalFile, string outDir, string fileName, object defaultContent)
        {
            if (extenalFile.IsNotEmpty())
            {
                return extenalFile;
            }
            else
            {
                var file = Path.Combine(outDir, fileName);

                if (defaultContent is byte[])
                    io.File.WriteAllBytes(file, (byte[])defaultContent);
                else if (defaultContent is Bitmap)
                    ((Bitmap)defaultContent).Save(file, ImageFormat.Png);
                else if (defaultContent is string)
                    io.File.WriteAllBytes(file, ((string)defaultContent).GetBytes());
                else if (defaultContent == null)
                    return "<null>";
                else
                    throw new Exception("Unsupported ManagedUI resource type.");

                Compiler.TempFiles.Add(file);
                return file;
            }
        }

        public static sys.Control LocalizeFrom(this sys.Control control, Func<string, string> localize)
        {
            var controls = new Queue<sys.Control>(new[] { control });

            while (controls.Any())
            {
                var item = controls.Dequeue();

                item.Text = item.Text.LocalizeFrom(localize);

                item.Controls
                    .OfType<sys.Control>()
                    .ForEach(x => controls.Enqueue(x));
            }
            return control;
        }


        static Regex locRegex = new Regex(@"\[.+?\]");
        static Regex cleanRegex = new Regex(@"{\\(.*?)}"); //removes font info "{\WixUI_Font_Bigger}Welcome to the [ProductName] Setup Wizard"

        public static string LocalizeFrom(this string textToLocalize, Func<string, string> localize)
        {
            if (textToLocalize.IsEmpty()) return textToLocalize;


            var result = new StringBuilder(textToLocalize);

            //first rum will replace all UI constants, which in turn may contain MSI properties to resolve.
            //second run will resolve properties if any found.
            for (int i = 0; i < 2; i++)
            {
                string text = result.ToString();
                result.Length = 0; //clear

                int lastEnd = 0;
                foreach (Match match in locRegex.Matches(text))
                {
                    result.Append(text.Substring(lastEnd, match.Index - lastEnd));
                    lastEnd = match.Index + match.Length;

                    string key = match.Value.Trim('[', ']');

                    result.Append(localize(key));
                }

                if (lastEnd != text.Length)
                    result.Append(text.Substring(lastEnd, text.Length - lastEnd));
            }
            return cleanRegex.Replace(result.ToString(), "");
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