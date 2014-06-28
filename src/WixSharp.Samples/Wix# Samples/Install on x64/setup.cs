//css_ref ..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using WixSharp;
using System.Xml.Linq;
using System.Xml;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
            new Project("MyProduct",
                new Dir(@"%ProgramFiles64Folder%\My Company\My Product",
                    new File(@"Files\Bin\MyApp.exe") { AttributesDefinition = "Component:Win64=yes" },
                    new Dir(@"Docs\Manual",
                        new File(@"Files\Docs\Manual.txt") { AttributesDefinition = "Component:Win64=yes" })));

        project.Package.AttributesDefinition = "Platform=x64";
        project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");

        //Alternatively you can set Component Attribute for all files together (do not forget to remove "Component:Win64=yes" from file's AttributesDefinition)
        
        //either before XML generation
        //foreach (var file in project.AllFiles)
        //    file.Attributes.Add("Component:Win64", "yes");

        //or do it as a post-generation step
        //Compiler.WixSourceGenerated += new XDocumentGeneratedDlgt(Compiler_WixSourceGenerated);

        Compiler.BuildMsi(project);
    }

    static void Compiler_WixSourceGenerated(XDocument document)
    {
        foreach (XElement comp in document.Root.AllElements("Component"))
            comp.Add(new XAttribute("Win64", "yes"));
    }
}



