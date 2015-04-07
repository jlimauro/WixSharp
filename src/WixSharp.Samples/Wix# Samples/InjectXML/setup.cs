//css_ref ..\..\WixSharp.dll;
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
                new Dir(@"%ProgramFiles%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe"),
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt"))));

        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        Compiler.WixSourceGenerated += document =>
                                       document.Descendants("Components")
                                               .ForEach(e => e.SetAttributeValue("Win64", "yes"));
        
        Compiler.WixSourceGenerated += InjectImages;


        Compiler.BuildMsi(project);
    }

    static void InjectImages(System.Xml.Linq.XDocument document)
    {
        var productElement = document.Root.Select("Product");

        productElement.Add(new XElement("WixVariable",
                               new XAttribute("Id", "WixUIBannerBmp"),
                               new XAttribute("Value", @"Images\bannrbmp.bmp")));

        productElement.Add(new XElement("WixVariable",
                               new XAttribute("Id", "WixUIDialogBmp"),
                               new XAttribute("Value", @"Images\dlgbmp.bmp")));
    }
}



