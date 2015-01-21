#region Licence...

/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted,
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#endregion Licence...

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IO = System.IO;

namespace WixSharp
{
    internal static class AutoElements
    {
        /// <summary>
        /// The disable automatic insertion of <c>CreateFolder</c> element.
        /// Required for: NativeBootstrapper, EmbeddedMultipleActions,  EmptyDirectories, InstallDir, Properties,
        /// ReleaseFolder, Shortcuts and WildCardFiles samples.
        /// </summary>
        public static bool DisableAutoCreateFolder = false;

        /// <summary>
        /// The disable automatic insertion of user profile registry elements.
        /// Required for: AllInOne, ConditionalInstallation, CustomAttributes, ReleaseFolder, Shortcuts,
        /// Shortcuts (advertised), Shortcuts-2, WildCardFiles samples.
        /// </summary>
        public static bool DisableAutoUserProfileRegistry = false;

        static void InsertRemoveFolder(XElement xDir, XElement xComponent, string when = "uninstall")
        {
            xComponent.Add(new XElement("RemoveFolder",
                               new XAttribute("Id", xDir.Attribute("Id").Value),
                               new XAttribute("On", when)));
        }

        static void InsertCreateFolder(XElement xDir, XElement xComponent)
        {
            //"Empty Directories" sample demonstrates the need for CreateFolder
            if (!DisableAutoCreateFolder)
                xComponent.Add(new XElement("CreateFolder"));
        }

        static void InsertDummyUserProfileRegistry(XElement xComponent)
        {
            if (!DisableAutoUserProfileRegistry)
                xComponent.Add(
                            new XElement("RegistryKey",
                                new XAttribute("Root", "HKCU"),
                                new XAttribute("Key", @"Software\WixSharp\Used"),
                                new XElement("RegistryValue",
                                    new XAttribute("Value", "0"),
                                    new XAttribute("Type", "string"),
                                    new XAttribute("KeyPath", "yes"))));
        }

        static void SetFileKeyPath(XElement element)
        {
            if (element.Attribute("KeyPath") == null)
                element.Add(new XAttribute("KeyPath", "yes"));
        }

        static bool ContainsDummyUserProfileRegistry(this XElement xComponent)
        {
            return (from e in xComponent.Elements("RegistryKey")
                    where e.Attribute("Key") != null && e.Attribute("Key").Value == @"Software\WixSharp\Used"
                    select e).Count() != 0;
        }

        static bool ContainsAnyRemoveFolder(this XElement xDir)
        {
            return (xDir.Descendants("RemoveFolder").Count() != 0);
        }

        static bool ContainsFiles(this XElement xComp)
        {
            return xComp.Elements("File").Count() != 0;
        }

        static bool ContainsComponents(this XElement xComp)
        {
            return xComp.Elements("Component").Count() != 0;
        }

        static bool ContainsAdvertisedShortcuts(this XElement xComp)
        {
            var advertisedShortcuts = from e in xComp.Descendants("Shortcut")
                                      where e.Attribute("Advertise") != null && e.Attribute("Advertise").Value == "yes"
                                      select e;

            return (advertisedShortcuts.Count() != 0);
        }

        static bool ContainsNonAdvertisedShortcuts(this XElement xComp)
        {
            var nonAdvertisedShortcuts = from e in xComp.Descendants("Shortcut")
                                         where e.Attribute("Advertise") == null || e.Attribute("Advertise").Value == "no"
                                         select e;

            return (nonAdvertisedShortcuts.Count() != 0);
        }

        static XElement CrteateComponentFor(this XDocument doc, XElement xDir)
        {
            string compId = xDir.Attribute("Id").Value;
            XElement xComponent = xDir.AddElement(
                              new XElement("Component",
                                  new XAttribute("Id", compId),
                                  new XAttribute("Guid", WixGuid.NewGuid(compId))));

            foreach (XElement xFeature in doc.Root.Descendants("Feature"))
                xFeature.Add(new XElement("ComponentRef",
                    new XAttribute("Id", xComponent.Attribute("Id").Value)));

            return xComponent;
        }

        private static string[] GetUserProfileFolders()
        {
            return new[]
                    {
                        "ProgramMenuFolder",
                        "AppDataFolder",
                        "LocalAppDataFolder",
                        "TempFolder",
                        "DesktopFolder"
                    };
        }

        static bool InUserProfile(this XElement xDir)
        {
            string[] userProfileFolders = GetUserProfileFolders();

            XElement xParentDir = xDir;
            do
            {
                if (xParentDir.Name == "Directory")
                {
                    var attrName = xParentDir.Attribute("Name").Value;

                    if (userProfileFolders.Contains(attrName))
                        return true;
                }
                xParentDir = xParentDir.Parent;
            }
            while (xParentDir != null);

            return false;
        }

        internal static void InjectShortcutIcons(XDocument doc)
        {
            var shortcuts = from s in doc.Root.Descendants("Shortcut")
                            where s.HasAttribute("Icon")
                            select s;

            int iconIndex = 1;

            var icons = new Dictionary<string, string>();
            foreach (var iconFile in (from s in shortcuts
                                      select s.Attribute("Icon").Value).Distinct())
            {
                icons.Add(iconFile,
                    "IconFile" + (iconIndex++) + "_" + IO.Path.GetFileName(iconFile).Expand());
            }

            foreach (XElement shortcut in shortcuts)
            {
                string iconFile = shortcut.Attribute("Icon").Value;
                string iconId = icons[iconFile];
                shortcut.Attribute("Icon").Value = iconId;
            }

            XElement product = doc.Root.Select("Product");

            foreach (string file in icons.Keys)
                product.AddElement(
                    new XElement("Icon",
                        new XAttribute("Id", icons[file]),
                        new XAttribute("SourceFile", file)));
        }

        internal static void InjectAutoElementsHandler(XDocument doc)
        {
            InjectShortcutIcons(doc);

            XElement installDir = doc.Root.Select("Product").Element("Directory").Element("Directory");

            XAttribute installDirName = installDir.Attribute("Name");
            if (IO.Path.IsPathRooted(installDirName.Value))
            {
                var product = installDir.Parent("Product");
                string absolutePath = installDirName.Value;

                installDirName.Value = "ABSOLUTEPATH";

                //<SetProperty> is an attractive approach but it doesn't allow conditional setting of 'ui' and 'execute' as required depending on UI level
                // it is ether hard coded 'both' or hard coded both 'ui' or 'execute' 
                // <SetProperty Id="INSTALLDIR" Value="C:\My Company\MyProduct" Sequence="both" Before="AppSearch">

                product.Add(new XElement("CustomAction",
                                new XAttribute("Id", "Set_INSTALLDIR_AbsolutePath"),
                                new XAttribute("Property", installDir.Attribute("Id").Value),
                                new XAttribute("Value", absolutePath)));

                product.SelectOrCreate("InstallExecuteSequence").Add(
                       new XElement("Custom", "(NOT Installed) AND (UILevel < 5)",
                           new XAttribute("Action", "Set_INSTALLDIR_AbsolutePath"),
                           new XAttribute("Before", "CostFinalize")));

                product.SelectOrCreate("InstallUISequence").Add(
                      new XElement("Custom", "(NOT Installed) AND (UILevel = 5)",
                          new XAttribute("Action", "Set_INSTALLDIR_AbsolutePath"),
                          new XAttribute("Before", "CostFinalize")));
            }

            foreach (XElement xDir in doc.Root.Descendants("Directory"))
            {
                var dirComponents = xDir.Elements("Component");

                foreach (XElement xComp in dirComponents)
                {
                    if (!xComp.ContainsFiles())
                    {
                        if (xDir.Attribute("Name").Value != "DummyDir")
                            InsertCreateFolder(xDir, xComp);
                        else if (!xDir.ContainsAnyRemoveFolder())
                            InsertRemoveFolder(xDir, xComp, "both"); //to keep WiX/compiler happy and allow removal of the dummy directory
                    }

                    if (xDir.InUserProfile())
                    {
                        if (!xDir.ContainsAnyRemoveFolder())
                            InsertRemoveFolder(xDir, xComp);

                        if (!xComp.ContainsDummyUserProfileRegistry())
                            InsertDummyUserProfileRegistry(xComp);
                    }
                    else
                    {
                        if (xComp.ContainsNonAdvertisedShortcuts())
                            if (!xComp.ContainsDummyUserProfileRegistry())
                                InsertDummyUserProfileRegistry(xComp);
                    }

                    foreach (XElement xFile in xComp.Elements("File"))
                        if (xFile.ContainsAdvertisedShortcuts())
                            SetFileKeyPath(xFile);
                }

                if (!xDir.ContainsComponents() && xDir.InUserProfile())
                {
                    XElement xComp1 = doc.CrteateComponentFor(xDir);
                    if (!xDir.ContainsAnyRemoveFolder())
                        InsertRemoveFolder(xDir, xComp1);

                    if (!xComp1.ContainsDummyUserProfileRegistry())
                        InsertDummyUserProfileRegistry(xComp1);
                }
            }
        }
    }
}