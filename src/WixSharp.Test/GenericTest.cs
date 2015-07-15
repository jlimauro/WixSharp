using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Xunit;
using WixSharp;
using io = System.IO;
using WixSharp.UI;

namespace WixSharp.Test
{
    public class GenericTest
    {
        [Fact]
        public void RelativePath()
        {
            var path = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Content\readme.txt";
            var baseDir = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Install Files";

            var result = Utils.MakeRelative(path, baseDir);
            Assert.Equal(@"..\Content\readme.txt", result);
        }

        [Fact]
        public void MsiParser_InstallDir()
        {
            //var parser = new MsiParser(@"..\WixSharp.Samples\Wix# Samples\Managed Setup\ManagedSetup.msi");
            string msi = @"E:\Galos\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Shortcuts\setup.msi";
            var parser = new MsiParser(msi);
            string installDirProperty = "INSTALLDIR";
            string dir = parser.GetDirectoryPath(installDirProperty);
        }

        [Fact]
        public void EnumExtensions()
        {
            var test = ConfigureServiceTrigger.Install | ConfigureServiceTrigger.Reinstall;

            Assert.True(ConfigureServiceTrigger.Install.PresentIn(test));
            Assert.False(ConfigureServiceTrigger.Uninstall.PresentIn(test));
        }

        [Fact]
        public void ArrayExtensions()
        {
            var project = new Project();
            project.Actions = new Action[] { new ManagedAction() };
            //should not throw
            project.Actions = project.Actions.Add(new InstalledFileAction("", ""));
            project.Actions = project.Actions.AddRange(project.Actions);
        }

        [Fact]
        public void Test()
        {
            Compiler.GuidGenerator = GuidGenerators.Default;
            Compiler.GuidGenerator = GuidGenerators.Sequential;
            Compiler.GuidGenerator = (seed) => Guid.Parse("9e2974a1-9539-4c5c-bef7-80fc35b9d7b0");
            Compiler.GuidGenerator = (seed) => Guid.NewGuid();
        }

        //[Fact]
        public void FeaturesAPI()
        {
            //var installedPackage = new Microsoft.Deployment.WindowsInstaller.ProductInstallation("{A6801CC8-AC2A-4BF4-BEAA-6EE4DCF17056}");
            var installedPackage = new Microsoft.Deployment.WindowsInstaller.ProductInstallation("A6801CC8-AC2A-4BF4-BEAA-6EE4DCF17056");
            if (!installedPackage.IsInstalled)
            {
            }
            else
            {
                foreach (var currentInstallFeature in installedPackage.Features)
                {
                    if (currentInstallFeature.State == InstallState.Local)
                    {
                        Debug.WriteLine(string.Format("Migrating feature {0} - marking as Present", currentInstallFeature.FeatureName));

                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Migrating feature {0} - marking as Absent", currentInstallFeature.FeatureName));
                    }
                }
            }
        }

        [Fact]
        public void Shoud_Resolve_WixVars()
        {
            var adminToolsFolder = "AdminToolsFolder".AsWixVarToPath();
            var appDataFolder = "AppDataFolder".AsWixVarToPath();
            var commonAppDataFolder = "CommonAppDataFolder".AsWixVarToPath();
            var commonFiles64Folder = "CommonFiles64Folder".AsWixVarToPath();
            var commonFilesFolder = "CommonFilesFolder".AsWixVarToPath();
            var desktopFolder = "DesktopFolder".AsWixVarToPath();
            var favoritesFolder = "FavoritesFolder".AsWixVarToPath();
            var programFiles64Folder = "ProgramFiles64Folder".AsWixVarToPath();
            var programFilesFolder = "ProgramFilesFolder".AsWixVarToPath();
            var myPicturesFolder = "MyPicturesFolder".AsWixVarToPath();
            var sendToFolder = "SendToFolder".AsWixVarToPath();
            var localAppDataFolder = "LocalAppDataFolder".AsWixVarToPath();
            var personalFolder = "PersonalFolder".AsWixVarToPath();
            var startMenuFolder = "StartMenuFolder".AsWixVarToPath();
            var startupFolder = "StartupFolder".AsWixVarToPath();
            var programMenuFolder = "ProgramMenuFolder".AsWixVarToPath();
            var system16Folder = "System16Folder".AsWixVarToPath();
            var system64Folder = "System64Folder".AsWixVarToPath();
            var systemFolder = "SystemFolder".AsWixVarToPath();
            var templateFolder = "TemplateFolder".AsWixVarToPath();
            var windowsVolume = "WindowsVolume".AsWixVarToPath();
            var windowsFolder = "WindowsFolder".AsWixVarToPath();
            var fontsFolder = "FontsFolder".AsWixVarToPath();
            var tempFolder = "TempFolder".AsWixVarToPath();

            Func<string, string, bool> isValid = (dir, ending) => io.Directory.Exists(dir) && dir.EndsWith(ending, StringComparison.OrdinalIgnoreCase);

            //expected to be tested on OS Vista or above from the x86 runtime
            Assert.True(isValid(adminToolsFolder, "Administrative Tools"));
            Assert.True(isValid(appDataFolder, @"AppData\Roaming"));
            Assert.True(isValid(commonAppDataFolder, "ProgramData"));
            Assert.True(isValid(commonFiles64Folder, "Common Files"));
            Assert.True(isValid(commonFilesFolder, "Common Files"));
            Assert.True(isValid(desktopFolder, "Desktop"));
            Assert.True(isValid(favoritesFolder, "Favorites"));
            Assert.True(isValid(programFiles64Folder, "Program Files"));
            Assert.True(isValid(programFilesFolder, "Program Files (x86)"));
            Assert.True(isValid(myPicturesFolder, "Pictures"));
            Assert.True(isValid(sendToFolder, "SendTo"));
            Assert.True(isValid(localAppDataFolder, "Local"));
            Assert.True(isValid(personalFolder, "Documents"));
            Assert.True(isValid(startMenuFolder, "Start Menu"));
            Assert.True(isValid(startupFolder, "Startup"));
            Assert.True(isValid(programMenuFolder, "Programs"));
            Assert.True(isValid(system16Folder, "system"));
            Assert.True(isValid(system64Folder, "system32"));
            Assert.True(isValid(systemFolder, "SysWow64"));
            Assert.True(isValid(templateFolder, "Templates"));
            Assert.True(isValid(windowsVolume, @"C:\"));
            Assert.True(isValid(windowsFolder, @"C:\Windows"));
            Assert.True(isValid(fontsFolder, @"C:\Windows\Fonts"));
            Assert.True(isValid(tempFolder, "Temp"));
        }

        [Fact]
        public void Should_Compare_CollectionByItems()
        {
            var itemsA = new[] { "a", "b" };
            var itemsC = new[] { "b", "c" };
            var itemsB = new List<string>(new[] { "a", "b" });

            Assert.Equal(itemsB.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(itemsC.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(new[] { "a" }.GetItemsHashCode(), itemsA.GetItemsHashCode());
        }

        [Fact]
        public void Should_Combine_Sequences()
        {
            var s1 = Sequence.InstallUISequence;
            var s2 = Sequence.InstallExecuteSequence;

            var result1 = s1 + s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result1.ToString());

            var result2 = s1 | s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result2.ToString());
        }

        [Fact]
        void StringEnum_Test()
        {
            Assert.Equal("newVal", new MyVals("newVal"));
            Assert.Equal("firstVal", MyVals.First);
            Assert.Equal("secondVal", MyVals.Second);
            Assert.Equal("thirdVal", MyVals.Third);

            MyVals empty = null;
            Assert.Equal(null, empty);
            Assert.True(empty == null);
            Assert.True(empty != MyVals.First);


            Assert.True(MyVals.Third == "thirdVal");
            Assert.True("thirdVal" == MyVals.Third);

            Assert.False(MyVals.Third == "");
            Assert.True(MyVals.Third != "");

            Assert.False("" == MyVals.Third);
            Assert.True("" != MyVals.Third);

            Assert.False(new MyVals("test") != new MyVals("test"));
            Assert.True(new MyVals("test") == new MyVals("test"));

            Assert.Equal("thirdVal", MyVals.Third);
            Assert.Equal("thirdVal", (string)MyVals.Third);

            Assert.True(MyVals.Third.Equals("thirdVal"));
            Assert.False(MyVals.Third.Equals(null));
            Assert.True(MyVals.Third.Equals(new MyVals("thirdVal")));
            Assert.True(MyVals.Third.Equals(MyVals.Third));
        }

        class MyVals : StringEnum<MyVals>
        {
            public MyVals(string value) : base(value) { }

            public static MyVals First = new MyVals("firstVal");
            public static MyVals Second = new MyVals("secondVal");
            public static MyVals Third = new MyVals("thirdVal");
        }
    }
}
