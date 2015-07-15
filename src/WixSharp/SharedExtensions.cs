using System;
using System.IO;

namespace WixSharp
{
    public static partial class Extensions
    {
        public static bool Is64OS()
        {
            //cannot use Environment.Is64BitOperatingSystem class as it is v3.5
            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string progFiles32 = progFiles;
            if (!progFiles32.EndsWith(" (x86)"))
                progFiles32 += " (x86)";

            return Directory.Exists(progFiles32);
        }

        //will always be called from x86 runtime as MSI always loads ManagedUI in x86 host.
        //Though CustomActions are called in the deployment specific CPU type context.
        public static string AsWixVarToPath(this string path)
        {
            switch (path)
            {
                case "AdminToolsFolder": return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Start Menu\Programs\Administrative Tools");

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

                case "System16Folder": return Path.Combine("WindowsFolder".AsWixVarToPath(), "System");
                case "System64Folder": return Environment.GetFolderPath(Environment.SpecialFolder.System);
                case "SystemFolder": return Is64OS() ? Path.Combine("WindowsFolder".AsWixVarToPath(), "SysWow64") : Environment.GetFolderPath(Environment.SpecialFolder.System);

                case "TemplateFolder": return Environment.GetFolderPath(Environment.SpecialFolder.Templates);
                case "WindowsVolume": return Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                case "WindowsFolder": return Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System));
                case "FontsFolder": return Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)), "Fonts");
                case "TempFolder": return Path.Combine(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), @"Local Settings\Temp");
                default:
                    return path;
            }
        }
    }
}