//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
                new Project("My Product",
                    new Dir(@"%ProgramFiles%\MyCompany",
                        new Dir(@"MyWebApp",
                            new File(@"MyWebApp\Default.aspx",
                                new IISVirtualDir
                                {
                                    Name = "MyWebApp",
                                    AppName = "Test",
                                    WebSite = new WebSite("DefaultWebSite", "*:80", "Default Web Site")
									//uncomment and use the line below (instead above one) if you want the new WebSite to be created instead of existing one to be reused
									//WebSite = new WebSite("MyWebSite", "*:90", "My Web Site") { InstallWebSite = true } 
                                }),
                            new File(@"MyWebApp\Default.aspx.cs"),
                            new File(@"MyWebApp\Default.aspx.designer.cs"),
                            new File(@"MyWebApp\Web.config"))));

        project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");
        project.UI = WUI.WixUI_ProgressOnly;
        project.MSIFileName = "setup";

        Compiler.BuildMsi(project);
    }
}



