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
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// Defines all files of a given source directory and all subdirectories to be installed on target system.
    /// <para>
    /// Use this class to define files to be automatically included into the deployment solution
    /// if their name matches specified wildcard character pattern (<see cref="Files.IncludeMask"/>).
    /// </para>
    /// <para>
    /// You can use <see cref="Files.ExcludeMasks"/> to exclude certain files from setup if required.
    /// </para>
    /// <para>
    /// This class is a logical equivalent of <see cref="DirFiles"/> except it also analyses all files in all subdirectories.
    /// <see cref="DirFiles"/> excludes files in subdirectories.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Note that all files matching wildcard are resolved into absolute path thus it may not always be suitable 
    /// if the Wix# script is to be compiled into WiX XML source only (Compiler.<see cref="WixSharp.Compiler.BuildWxs(WixSharp.Project)"/>). Though it is not a problem at all if the Wix# script 
    /// is compiled into MSI file (Compiler.<see cref="Compiler.BuildMsi(WixSharp.Project)"/>).
    /// </remarks>b
    /// <example>The following is an example of defining installation files with wildcard character pattern.
    /// <code>
    /// new Project("MyProduct",
    ///     new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
    ///         new Files(@"Release\Bin\*.*"),
    ///         ...
    /// </code>
    /// </example>
    public partial class Files : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Files"/> class.
        /// </summary>
        public Files() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Files"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new Files(@"Release\Bin\*.*")</c>).</param>
        public Files(string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Files"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">The relative path to source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new Files(@"Release\Bin\*.*")</c>).</param>
        /// <param name="excludeFileMasks">Wildcard pattern(s) for files to be excluded from MSI 
        /// (e.g. <c>new Files(typical, @"Release\Bin\*.dll", "*.Test.dll", "*.UnitTest.dll")</c>).</param>
        public Files(string sourcePath, params string[] excludeFileMasks)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            ExcludeMasks = excludeFileMasks;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Files"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new Files(@"Release\Bin\*.*")</c>).</param>
        public Files(Feature feature, string sourcePath)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            Feature = feature;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Files"/> class with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the directory files should be included in.</param>
        /// <param name="sourcePath">The relative path to source directory. It must include wildcard pattern for files to be included
        /// into MSI (e.g. <c>new Files(@"Release\Bin\*.*")</c>).</param>
        /// <param name="excludeFileMasks">Wildcard pattern(s) for files to be excluded from MSI 
        /// (e.g. <c>new Files(typical, @"Release\Bin\*.dll", "*.Test.dll", "*.UnitTest.dll")</c>).</param>
        public Files(Feature feature, string sourcePath, params string[] excludeFileMasks)
        {
            IncludeMask = IO.Path.GetFileName(sourcePath);
            Directory = IO.Path.GetDirectoryName(sourcePath);
            ExcludeMasks = excludeFileMasks;
            Feature = feature;
        }

        /// <summary>
        /// <see cref="Feature"></see> the directory files are included in.
        /// </summary>
        public Feature Feature;
        /// <summary>
        /// The relative path to source directory to search for files matching the <see cref="Files.IncludeMask"/>.
        /// </summary>
        public string Directory = "";
        /// <summary>
        /// Wildcard pattern for files to be included into MSI. 
        /// <para>Default value is <c>*.*</c>.</para>
        /// </summary>
        public string IncludeMask = "*.*";


        /// <summary>
        /// Wildcard patterns for files to be excluded from MSI.
        /// </summary>
        public string[] ExcludeMasks = new string[0];

        /// <summary>
        /// Analyses <paramref name="baseDirectory"/> and returns all files (including subdirectories) matching <see cref="Files.IncludeMask"/>,
        /// which are not matching any <see cref="Files.ExcludeMasks"/>.
        /// </summary>
        /// <param name="baseDirectory">The base directory for file analysis. It is used in conjunction with 
        /// relative <see cref="DirFiles.Directory"/>.</param>
        /// <returns>Array of <see cref="WixEntity"/> instances, which are either <see cref="File"/> or/and <see cref="Dir"/> objects.</returns>
        public WixEntity[] GetAllItems(string baseDirectory)
        {
            //Func<string, string> ToRelativePath = delegate(string dirPath)
            //                     {
            //                         return dirPath.Substring(baseDirectory.Length);
            //                     };

            if (baseDirectory.IsEmpty())
                baseDirectory = Environment.CurrentDirectory;

            Action<Dir, string> AgregateSubDirs = null;
            AgregateSubDirs = delegate(Dir parentDir, string dirPath)
                                {
                                    foreach (var subDirPath in IO.Directory.GetDirectories(dirPath))
                                    {
                                        var dirName = IO.Path.GetFileName(subDirPath);
                                        var subDir = new Dir(dirName, new DirFiles(Feature, ToRelativePath(subDirPath, baseDirectory) + "\\" + IncludeMask, ExcludeMasks));
                                        AgregateSubDirs(subDir, subDirPath);

                                        parentDir.Dirs = parentDir.Dirs.Add<Dir>(subDir);
                                    }
                                };

            var items = new List<WixEntity> { new DirFiles(Feature, Utils.PathCombine(Directory, IncludeMask), ExcludeMasks) };

            string rootDirPath = Utils.PathCombine(baseDirectory, Directory);

            if (!IO.Directory.Exists(rootDirPath))
                throw new IO.DirectoryNotFoundException(rootDirPath);

            foreach (var subDirPath in System.IO.Directory.GetDirectories(rootDirPath))
            {
                var dirName = IO.Path.GetFileName(subDirPath);
                var subDir = new Dir(dirName, new DirFiles(Feature, ToRelativePath(subDirPath, baseDirectory) + "\\" + IncludeMask, ExcludeMasks));
                AgregateSubDirs(subDir, subDirPath);

                items.Add(subDir);
            }

            return items.ToArray();
        }

        static internal string ToRelativePath(string filePath, string baseDirPath)
        {
            string path = filePath;
            string baseDir = baseDirPath;

            if (baseDirPath.Length == 0)
            {
                return filePath;
            }

            if (baseDir[baseDir.Length - 1] == IO.Path.DirectorySeparatorChar)
                baseDir = baseDir.Substring(0, baseDir.Length - 1);

            var relativeDir = IO.Path.GetDirectoryName(path).Substring(baseDir.Length);

            if (!string.IsNullOrEmpty(relativeDir))
                if (relativeDir[0] == IO.Path.DirectorySeparatorChar)
                    relativeDir = relativeDir.Substring(1);

            return Utils.PathCombine(relativeDir, IO.Path.GetFileName(path));
        }

        static void Test(string[] args)
        {
            var ttt = ToRelativePath(@"C:\tt\gg\yy\x", @"C:\tt\gg\yy\");
            //ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:\tt\gg\yy\test.txt");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:\tt\gg\yy");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:\tt\");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:\tt");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:\");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"C:");
            ttt = ToRelativePath(@"C:\tt\gg\yy\test.txt", @"");
            ttt = ToRelativePath(@"gg\yy\test.txt", @"gg\yy\");
            ttt = ToRelativePath(@"gg\yy\test.txt", @"gg\yy");
            ttt = ToRelativePath(@"gg\yy\test.txt", @"gg\");
            ttt = ToRelativePath(@"gg\yy\test.txt", @"gg");
            ttt = ToRelativePath(@"\\gg\yy\test.txt", @"\\gg\yy\");
            ttt = ToRelativePath(@"\\gg\yy\test.txt", @"\\gg\yy");
            ttt = ToRelativePath(@"\\gg\yy\test.txt", @"\\gg\");
            ttt = ToRelativePath(@"\\gg\yy\test.txt", @"\\gg");
        }

    }
}
