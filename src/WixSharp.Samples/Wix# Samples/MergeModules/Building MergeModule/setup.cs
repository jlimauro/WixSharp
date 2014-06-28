//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;

using System;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Project project =
               new Project("MyMergeModule",
                   new Dir(@"%ProgramFiles%\My Company",
                       new File(@"File1.txt"),
                       new File(@"File2.txt")));

        project.UI = WUI.WixUI_ProgressOnly;

        var msm = Compiler.BuildMsm(project);
    }
}
