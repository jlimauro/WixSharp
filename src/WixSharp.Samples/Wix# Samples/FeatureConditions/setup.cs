//css_dir ..\..\;
//css_ref Wix_bin\SDK\Microsoft.Deployment.WindowsInstaller.dll;
//css_ref System.Core.dll;

using System;
using File = WixSharp.File;
using WixSharp;
using System.Collections.Generic;

class Script
{

    /// <summary>
    /// The following project has 2 Features. FeatureA is a normal Feature and will always be installed (for installations with default values).
    /// FeatureB is a Feature defined with a Condition. The Condition below will install FeatureB if and only if an install property 'Prop1' has a value
    /// equal to 1. If Prop1 has any other value, then FeatureB will not be installed alongside FeatureA. 
    /// </summary>
    /// <param name="args"></param>
    static public void Main(string[] args)
    {
        //featureA - a normal feature
        var featureA = new Feature("Feature A");

        //featureB - a conditional feature
        var featureB = new Feature("Feature B");

        //Note - Level can be set explicitly via Attributes or indirectly via IsEnabled
        //featureB.Attributes =
        //    new Dictionary<string, string>
        //        {
        //            { "Level", "2" }
        //        };
        featureB.IsEnabled = false;

        //if the condition evaluates to true - the level of the parent feature is updated to the level of the FeatureCondition
        featureB.Condition = new FeatureCondition("PROP1 = 1", level: 1);

        var project =
            new Project("FeatureCondition",
                new Dir(@"%ProgramFiles%\My Company\Features",
                    new File(featureA, @"Files\MainFile.txt"),
                    new File(featureB, @"Files\SecondaryFile.txt")));

        project.UI = WUI.WixUI_FeatureTree;

        project.LaunchConditions.Add(new LaunchCondition("PROP1", "PROP1 is required"));
        project.PreserveTempFiles = true;

        Compiler.BuildMsi(project);
    }
}
