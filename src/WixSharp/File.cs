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

using IO = System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WixSharp
{

    /// <summary>
    /// Defines file to be installed.
    /// </summary>
    /// 
    ///<example>The following is an example of installing <c>MyApp.exe</c> file.
    ///<code>
    /// var project = 
    ///     new Project("My Product",
    ///     
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///                 new WixSharp.Shortcut("MyApp", @"%ProgramMenu%\My Company\My Product"),
    ///                 new WixSharp.Shortcut("MyApp", @"%Desktop%")),
    ///                 
    ///         ...
    ///         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    public partial class File : WixEntity
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the <see cref="File"/>.
        /// <para>This property is designed to produce friendlier string representation of the <see cref="File"/>
        /// for debugging purposes.</para>
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the <see cref="File"/>.
        /// </returns>
        public new string ToString()
        {
            return IO.Path.GetFileName(Name) + "; " + Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public File() { }
        /// <summary>
        /// Creates instance of the <see cref="File"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="feature"><see cref="Feature"></see> the file should be included in.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the file to be created 
        /// during the installation.</param>
        public File(Feature feature, string sourcePath, params WixEntity[] items)
        {
            Name = sourcePath;
            Feature = feature;
            AddItems(items);
        }
        /// <summary>
        /// Creates instance of the <see cref="File"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="File"/> instance.</param>
        /// <param name="feature"><see cref="Feature"></see> the file should be included in.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the file to be created 
        /// during the installation.</param>
        public File(Id id, Feature feature, string sourcePath, params WixEntity[] items)
        {
            Id = id.Value;
            Name = sourcePath;
            Feature = feature;
            AddItems(items);
        }
        /// <summary>
        /// Creates instance of the <see cref="File"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the file to be created 
        /// during the installation.</param>
        public File(string sourcePath, params WixEntity[] items)
        {
            Name = sourcePath;
            AddItems(items);
        }
        /// <summary>
        /// Creates instance of the <see cref="File"></see> class with properties initialized with specified parameters.
        /// </summary>
        /// <param name="id">The explicit <see cref="Id"></see> to be associated with <see cref="File"/> instance.</param>
        /// <param name="sourcePath">Relative path to the file to be taken for building the MSI.</param>
        /// <param name="items">Optional <see cref="FileShortcut"/> parameters defining shortcuts to the file to be created 
        /// during the installation.</param>
        public File(Id id, string sourcePath, params WixEntity[] items)
        {
            Id = id.Value;
            Name = sourcePath;
            AddItems(items);
        }

        void AddItems(WixEntity[] items)
        {
            Shortcuts = (from i in items where i is FileShortcut select i as FileShortcut).ToArray();
            Associations = (from i in items where i is FileAssociation select i as FileAssociation).ToArray();
            IISVirtualDirs = (from i in items where i is IISVirtualDir select i as IISVirtualDir).ToArray();
            if ((Associations.Length + Shortcuts.Length + IISVirtualDirs.Length) != items.Length)
                throw new ApplicationException("Only {0} and {1} items can be added to {2}".Format(typeof(FileShortcut), typeof(FileAssociation), this.GetType()));
        }

        /// <summary>
        /// Collection of the <see cref="FileAssociation"/>s associated with the file. 
        /// </summary>
        public FileAssociation[] Associations = new FileAssociation[0];

        /// <summary>
        /// Collection of the contained <see cref="IISVirtualDir"/>s. 
        /// </summary>
        public IISVirtualDir[] IISVirtualDirs = new IISVirtualDir[0];
        
        /// <summary>
        /// Collection of the <see cref="Shortcut"/>s associated with the file. 
        /// </summary>
        public FileShortcut[] Shortcuts = new FileShortcut[0];
        /// <summary>
        /// <see cref="Feature"></see> the file is included in.
        /// </summary>
        public Feature Feature;
        /// <summary>
        /// Defines the installation <see cref="Condition"/>, which is to be checked during the installation to 
        /// determine if the file should be installed on the target system.
        /// </summary>
        public Condition Condition;
    }
}