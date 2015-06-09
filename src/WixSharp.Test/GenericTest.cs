using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using Xunit;

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
        [Description("Issue #37")]
        public void Should_Preserve_ConstantsInAttrDefs()
        {
            var project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\MyCompany",
                        new Dir("MyWebApp",
                            new File(@"MyWebApp\Default.aspx",
                            new IISVirtualDir
                            {
                                Name = "MyWebApp",
                                AppName = "Test",
                                WebSite = new WebSite("DefaultWebSite", "[IIS_SITE_ADDRESS]:[IIS_SITE_PORT]", "[IIS_SITE_NAME]"),
                                WebAppPool = new WebAppPool("MyWebApp", "Identity=applicationPoolIdentity")
                            }))));


            string wxs = project.BuildWxs();

            var address = XDocument.Load(wxs)
                                   .FindSingle("WebAddress");

            Assert.Equal("[IIS_SITE_ADDRESS]", address.ReadAttribute("Id"));
            Assert.Equal("[IIS_SITE_PORT]", address.ReadAttribute("Port"));
        }
        
        [Fact]
        [Description("Post 576142#post1428674")]
        public void Should_Handle_NonstandardProductVersions()
        {
            Project project = new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(this.GetType().Assembly.Location)
                )
            );

            project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
            project.Version = new Version("2014.1.26.0");

            Compiler.BuildMsi(project); 
        }

        void CodeFormattingTestPad()
        {
        }
    }

}
