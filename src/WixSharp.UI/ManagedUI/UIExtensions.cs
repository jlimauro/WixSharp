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

namespace WixSharp
{
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

        static bool Is64OS()
        {
            //cannot use Environment.Is64BitOperatingSystem class as it is v3.5
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string progFiles32 = progFiles;
            if (!progFiles32.EndsWith(" (x86)"))
                progFiles32 += " (x86)";

            return io.Directory.Exists(progFiles32);
        }


        //will always be called from x86 runtime as MSI always loads ManagedUI in x86 host.
        //Though CustomActions are called in the deployment specific CPU type context.
        public static string AsWixVarToPath(this string path)
        {
            switch (path)
            {
                case "AdminToolsFolder": return io.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Start Menu\Programs\Administrative Tools");

                case "AppDataFolder": return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                case "CommonAppDataFolder": return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                case "CommonFiles64Folder": return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles).Replace(" (x86)", "");
                case "CommonFilesFolder": return Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

                case "DesktopFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "FavoritesFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Favorites);

                case "ProgramFiles64Folder": return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Replace(" (x86)", "");
                case "ProgramFilesFolder": return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                case "MyPicturesFolder": return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                case "SendToFolder": return Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
                case "LocalAppDataFolder": return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                case "PersonalFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                case "StartMenuFolder": return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                case "StartupFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                case "ProgramMenuFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Programs);

                case "System16Folder": return io.Path.Combine("WindowsFolder".AsWixVarToPath(), "System");
                case "System64Folder": return Environment.GetFolderPath(Environment.SpecialFolder.System);
                case "SystemFolder": return Is64OS() ? io.Path.Combine("WindowsFolder".AsWixVarToPath(), "SysWow64") : Environment.GetFolderPath(Environment.SpecialFolder.System);

                case "TemplateFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                case "WindowsVolume": return io.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                case "WindowsFolder": return io.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System));
                case "FontsFolder": return io.Path.Combine(io.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)), "Fonts");
                case "TempFolder": return io.Path.Combine(io.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), @"Local Settings\Temp");
                default:
                    return path;
            }
        }

        public static string GetInstallDirectoryName(this Session session)
        {
            List<Dictionary<string, object>> result = session.OpenView("select * from Component", "Directory_");

            var dirs = result.Select(x=>x["Directory_"]).Cast<string>().Distinct().ToArray();

            string shortestDir = dirs.Select(x => new { Name = x, Parts = session.GetDirectoryPathParts(x) })
                                     .OrderBy(x => x.Parts.Length)
                                     .Select(x=>x.Name)
                                     .FirstOrDefault();
            if (shortestDir == null)
                throw new Exception("GetInstallDirectoryPath Error: cannot find InstallDirectory");
            else
                return shortestDir;
        }

public static string GetDirectoryPath(this Session session, string name)
{
    string[] subDirs = session.GetDirectoryPathParts(name)
                                .Select(x => x.AsWixVarToPath())
                                .ToArray();
    return string.Join(@"\", subDirs);
}

static string[] GetDirectoryPathParts(this Session session, string name)
{
    var path = new List<string>();
    var names = new Queue<string>(new[] { name });

    while (names.Any())
    {
        var item = names.Dequeue();

        using (var sql = session.Database.OpenView("select * from Directory where Directory = '" + item + "'"))
        {
            sql.Execute();
            using (var record = sql.Fetch())
            {
                //var _name = record["Directory"];
                var subDir = record.GetString("DefaultDir").Split('|').Last();
                path.Add(subDir);

                if (!record.IsNull("Directory_Parent"))
                {
                    var parent = record.GetString("Directory_Parent");
                    if (parent != "TARGETDIR")
                        names.Enqueue(parent);
                }
            }
        }
    }
    path.Reverse();
    return path.ToArray();
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
}