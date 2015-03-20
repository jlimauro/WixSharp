#region Licence...
/*
The MIT License (MIT)

Copyright (c) 2014 Oleg Shilo

Permission is hereby granted, 
free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WixSharp
{
    class ProjectValidator
    {
        static bool IsValidVersion(string versionText)
        {
            if (string.IsNullOrEmpty(versionText))
                return true;

            if (versionText == "%this%")
                return true;

            try
            {
                new Version(versionText);
                return true;
            }
            catch
            { return false; }
        }

        public static void Validate(Project project)
        {
            if (project.MajorUpgradeStrategy != null)
            {
                if (project.MajorUpgradeStrategy.UpgradeVersions == null && project.MajorUpgradeStrategy.PreventDowngradingVersions == null)
                {
                    throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions and PreventDowngradingVersions are not defined.");
                }

                if (project.MajorUpgradeStrategy.UpgradeVersions != null)
                {
                    if (!IsValidVersion(project.MajorUpgradeStrategy.UpgradeVersions.Minimum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions.Minimum value is invalid.");

                    if (!IsValidVersion(project.MajorUpgradeStrategy.UpgradeVersions.Maximum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.UpgradeVersions.Maximum value is invalid.");
                }

                if (project.MajorUpgradeStrategy.PreventDowngradingVersions != null)
                {
                    if (!IsValidVersion(project.MajorUpgradeStrategy.PreventDowngradingVersions.Minimum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.PreventDowngradingVersions.Minimum value is invalid.");

                    if (!IsValidVersion(project.MajorUpgradeStrategy.PreventDowngradingVersions.Maximum))
                        throw new UpgradeStrategyValidationException("Project MajorUpgradeStrategy.PreventDowngradingVersions.Maximum value is invalid.");
                }
            }

            foreach (Dir dir in project.AllDirs)
                if (dir.Name.StartsWith("%") || dir.Name.EndsWith("%"))
                    if (!Compiler.EnvironmentConstantsMapping.ContainsKey(dir.Name))
                        throw new ValidationException("WixSharp.Dir.Name is set to unknown environment constant '" + dir.Name + "'.\n" +
                                                      "For the list of supported constants analyze WixSharp.Compiler.EnvironmentConstantsMapping.Keys.");



            var incosnistentRefAsmActions =
                      project.Actions.OfType<ManagedAction>()
                                     .GroupBy(a => a.ActionAssembly)
                                     .Where(g => g.Count() > 1)
                                     .Select(g => new
                                     {
                                         Assembly = g.Key,
                                         //RefAsmHashes = g.Select(action => action.GetRefAssembliesHashCode(project.DefaultRefAssemblies)).ToArray(), //enable if needed for troubleshooting
                                         IsInconsistent = g.Select(action => action.GetRefAssembliesHashCode(project.DefaultRefAssemblies)).Distinct().Count() > 1,
                                     })
                                     .Where(x=>x.IsInconsistent)
                                     .FirstOrDefault();

            if (incosnistentRefAsmActions != null)
                throw new ApplicationException(string.Format("ManagedAction assembly '{0}' is declared multiple times with the different (inconsistent) set of referenced assemblies. "+
                                                             "Ensure that all declarations have the same referenced assemblies by either using identical declarations or by using "+
                                                             "Project.DefaultRefAssemblies.", incosnistentRefAsmActions.Assembly));
        }
    }
}
