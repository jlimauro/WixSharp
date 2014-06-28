//css_ref ..\..\..\WixSharp.dll;
//css_ref System.Core.dll;

using System;
using System.Windows.Forms;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        Feature featureA = new Feature("Feature A");
        Feature featureB = new Feature("Feature B");
        
        Project project =
               new Project("MyMergeModule",
                   new Dir(@"%ProgramFiles%\My Company",
                       new File(featureA, "MainFile.txt"),	
                       new Merge(featureB,"MyMergeModule.msm")));

        project.UI = WUI.WixUI_FeatureTree;

        Compiler.BuildMsi(project);
    }
}
