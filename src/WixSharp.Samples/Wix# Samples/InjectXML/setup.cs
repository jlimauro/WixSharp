//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        //Note: setting x64 is done via XML injection for demo purposes only.
        //The x64 install can be achieved by "project.Platform = Platform.x64;"

        //project specific build event
        project.WixSourceGenerated += InjectImages;

        //global build event
        Compiler.WixSourceGenerated += document =>
            {
                document.Root.Select("Product/Package")
                             .SetAttributeValue("Platform", "x64");

                document.Descendants("Component")
                        .ForEach(e => e.SetAttributeValue("Win64", "yes"));
            };

        Compiler.PreserveTempFiles = true;
        Compiler.BuildMsi(project);
    }

    static void InjectImages(System.Xml.Linq.XDocument document)
    {
        var productElement = document.Root.Select("Product");

        productElement.Add(new XElement("WixVariable",
                               new XAttribute("Id", "WixUIBannerBmp"),
                               new XAttribute("Value", @"Images\bannrbmp.bmp")));

        //alternative syntax
        productElement.AddElement("WixVariable", @"Id=WixUIDialogBmp;Value=Images\dlgbmp.bmp");
    }
}



