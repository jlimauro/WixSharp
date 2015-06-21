using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class IssueFixesTest
    {
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

        [Fact]
        [Description("Issue #39")]
        public void Should_Handle_EmptyFeatures()
        {
            var binaries = new Feature("MyApp Binaries");
            var docs = new Feature("MyApp Documentation");
            var docs_01 = new Feature("Documentation 01");
            var docs_02 = new Feature("Documentation 02");
            var docs_03 = new Feature("Documentation 03");

            docs.Children.Add(docs_01);
            docs.Children.Add(docs_02);
            docs.Children.Add(docs_03);

            binaries.Children.Add(docs);

            Project project = new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(binaries, @"Files\Bin\MyApp.exe"),
                    new Dir(docs, @"Docs\Manual",
                        new File(docs_01, @"Files\Docs\Manual_01.txt"),
                        new File(docs_02, @"Files\Docs\Manual_02.txt"),
                        new File(docs_03, @"Files\Docs\Manual_03.txt")
                    )
                )
            );

            project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

            var wsxfile = project.BuildWxs();

            var doc = XDocument.Load(wsxfile);

            var product = doc.FindSingle("Product");

            var rootFeature = doc.Select("Wix/Product/Feature");
            Assert.NotNull(rootFeature);

            var docsFeature = rootFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                           && e.HasAttribute("Title", value => value == "MyApp Documentation"));
            Assert.NotNull(docsFeature);

            var doc1Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                           && e.HasAttribute("Title", value => value == "Documentation 01"));
            Assert.NotNull(doc1Feature);

            var doc2Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                           && e.HasAttribute("Title", value => value == "Documentation 02"));
            Assert.NotNull(doc2Feature);

            var doc3Feature = docsFeature.Elements()
                                         .FirstOrDefault(e => e.HasLocalName("Feature")
                                                           && e.HasAttribute("Title", value => value == "Documentation 03"));
            Assert.NotNull(doc3Feature);


        }

        [Fact]
        [Description("Issue #49")]
        public void Should_Fix_Issue_49()
        {
            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"%ProgramFiles%",
                                  new Dir(@"AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };
                project.UI = WUI.WixUI_InstallDir;

                var msi = project.BuildMsi();
            }
         
            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"C:\",
                                  new Dir(@"Program Files (x86)\AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };
                project.UI = WUI.WixUI_InstallDir;

                var msi = project.BuildMsi();

                //var msi = project.BuildWxs();
            }
            {
                var project = new Project("MyProduct");

                var rootDir = new Dir(@"C:\Program Files (x86)",
                                  new Dir(@"AAA\BBB",
                                      new File(this.GetType().Assembly.Location)));

                project.Dirs = new[] { rootDir };

                project.BuildMsi();
            }

        }
    }
}