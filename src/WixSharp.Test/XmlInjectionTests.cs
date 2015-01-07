using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class TestData
    {
        public static string Path(string path="")
        {
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(@"..\..\TestData", path));
        }
    }

    class XmlInjectionTests
    {
        [Fact]
        public void Test()
        {
            Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

            project.AddRemoveProgramsIcon = TestData.Path(@"Files\app_icon.ico");
            project.Version = new Version("2015.0.41.0");
            project.SourceBaseDir = TestData.Path();

            //Compiler.WixSourceGenerated += Compiler_WixSourceGenerated;
            var wxs = Compiler.BuildMsi(project);
        }

        void Compiler_WixSourceGenerated(System.Xml.Linq.XDocument document)
        {
            var product = document.Root.Select("Product");
            product.AddElement("Icon", "Id=app_icon.ico;SourceFile="+TestData.Path(@"Files\app_icon.ico"));
            product.AddElement("Property", "Id=ARPPRODUCTICON;Value=app_icon.ico");
        }
    }
}