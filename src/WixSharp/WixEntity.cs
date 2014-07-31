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
using System.Diagnostics;

namespace WixSharp
{
    /// <summary>
    /// Base class for all Wix# related types
    /// </summary>
    public class WixObject
    {
    }

    /// <summary>
    /// Alias for the <c>Dictionary&lt;string, string&gt; </c> type.
    /// </summary>
    public class Attributes : Dictionary<string, string>
    {
    }
    /// <summary>
    /// Generic <see cref="T:WixSharp.WixEntity"/> container for defining WiX <c>Package</c> element attributes.
    /// <para>These attributes are the properties about the package to be placed in the Summary Information Stream. These are visible from COM through the IStream interface, and these properties can be seen on the package in Explorer. </para>
    ///<example>The following is an example of defining the <c>Package</c> attributes.
    ///<code>
    /// var project = 
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///     ...
    ///         
    /// project.Package.AttributesDefinition = @"AdminImage=Yes;
    ///                                          Comments=Release Candidate;
    ///                                          Description=Fantastic product...";
    ///                                         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class Package : WixEntity
    {
    }
    /// <summary>
    /// Generic <see cref="T:WixSharp.WixEntity"/> container for defining WiX <c>Media</c> element attributes.
    /// <para>These attributes describe a disk that makes up the source media for the installation.</para>
    ///<example>The following is an example of defining the <c>Media</c> attributes.
    ///<code>
    /// var project = 
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///         
    ///     ...
    ///         
    /// project.Media.AttributesDefinition = @"Id=2;
    ///                                        CompressionLevel=mszip";
    ///                                         
    /// Compiler.BuildMsi(project);
    /// </code>
    /// </example>
    /// </summary>
    public class Media : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Media"/> class.
        /// </summary>
        public Media()
        {
            AttributesDefinition = "Id=1;EmbedCab=yes";
        }
    }
    /// <summary>
    /// Base class for all Wix# types representing WiX XML elements (entities)
    /// </summary>
    public class WixEntity : WixObject
    {
        /// <summary>
        /// Collection of Attribute/Value pairs for WiX element attributes not supported directly by Wix# objects.
        /// <para>You should use <c>Attributes</c> if you want to inject specific XML attributes 
        /// for a given WiX element.</para>
        /// <para>For example <c>Hotkey</c> attribute is not supported by Wix# <see cref="T:WixSharp.Shortcut"/> 
        /// but if you want them to be set in the WiX source file you may achieve this be setting
        /// <c>WixEntity.Attributes</c> member variable:
        /// <para> <code>new Shortcut { Attributes= new { {"Hotkey", "0"} }</code> </para>
        /// <remarks>
        /// You can also inject attributes into WiX components "related" to the <see cref="WixEntity"/> but not directly 
        /// represented in the Wix# entities family. For example if you need to set custom attribute for the WiX <c>Component</c> 
        /// XML element you can use corresponding <see cref="T:WixSharp.File"/> attributes. The only difference comparing to 
        /// the <c>Hotkey</c> example is the composite (column separated) key name:
        /// <para> <code>new File { Attributes= new { {"Component:SharedDllRefCount", "yes"} }</code> </para>
        /// The code above will force the Wix# compiler to insert "SharedDllRefCount" attribute into <c>Component</c>
        /// XML element, which is automatically generated for the <see cref="T:WixSharp.File"/>. 
        /// <para>Currently the only supported "related" attribute is  <c>Component</c>.</para>
        /// </remarks>
        /// </para>
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get
            {
                ProcessAttributesDefinition();
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }
        Dictionary<string, string> attributes = new Dictionary<string, string>();

        /// <summary>
        /// Optional attributes of the <c>WiX Element</c> (e.g. Secure:YesNoPath). 
        /// <para>OptionalAttributes just redirects all access calls to the <see cref="T:WixEntity.Attributes"/> member.</para>
        /// <para>You can also use <see cref="T:WixEntity.AttributesDefinition"/> to keep the code cleaner.</para>
        /// </summary>
        /// <example>
        /// <code>
        /// var webSite = new WebSite
        /// {
        ///     Description = "MyWebSite",
        ///     Attributes = new Dictionary&lt;string, string&gt; { { "StartOnInstall", "Yes" },  { "Sequence", "1" } }
        ///     //or
        ///     AttributesDefinition = "StartOnInstall=Yes; Sequence=1"
        ///     ...
        /// </code>
        /// </example>
        public string AttributesDefinition { get; set; }
        void ProcessAttributesDefinition()
        {
            if (!AttributesDefinition.IsEmpty())
            {
                var attrToAdd = new Dictionary<string, string>();

                foreach (string attrDef in AttributesDefinition.Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        string[] tokens = attrDef.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string name = tokens[0].Trim();
                        string value = tokens[1].Trim();
                        attrToAdd.Add(name, value);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Invalid AttributesDefinition", e);
                    }
                }

                this.Attributes = attrToAdd;
            }
        }
        
        /// <summary>
        /// Returns all attributes of the <see cref="Attributes"/> directly associated with <see cref="WixEntity"/>.
        /// </summary>
        internal Dictionary<string, string> LocalAttributes
        {
            get
            {
                var retval = new Dictionary<string, string>();

                foreach (var key in Attributes.Keys)
                    if (!key.Contains(":"))
                        retval.Add(key, Attributes[key]);

                return retval;
            }
        }
        /// <summary>
        /// Returns all attributes of the <see cref="Attributes"/> associated with <see cref="WixEntity"/>
        /// "related" element XML <c>Component</c>.
        /// </summary>
        internal Dictionary<string, string> ComponentAttributes
        {
            get
            {
                var retval = new Dictionary<string, string>();

                foreach (var key in Attributes.Keys)
                    if (key.Contains("Component:"))
                        retval.Add(key.Replace("Component:", ""), Attributes[key]);

                return retval;
            }
        }
        /// <summary>
        /// Name of the <see cref="WixEntity"/>. 
        /// <para>This value is used as a <c>Name</c> for the corresponding WiX XML element.</para>
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Gets or sets the <c>Id</c> value of the <see cref="WixEntity"/>. 
        /// <para>This value is used as a <c>Id</c> for the corresponding WiX XML element.</para>
        /// <para>If the <see cref="Id"/> value is not specified explicitly by the user the Wix# compiler
        /// generates it automatically insuring it's uniqueness.</para>
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                if (id.IsEmpty())
                {
                    if (!idMaps.ContainsKey(GetType()))
                        idMaps[GetType()] = new Dictionary<string, int>();

                    var rawName = Name.Expand();

                    if (GetType() != typeof(Dir) && GetType().BaseType != typeof(Dir))
                        rawName = IO.Path.GetFileName(Name).Expand();

                    string rawNameKey = rawName.ToLower();

                    /*
                     "bin\Release\similarFiles.txt" and "bin\similarfiles.txt" will produce the following IDs
                     "Component.similarFiles.txt" and "Component.similariles.txt", which will be treated by Wix compiler as duplication 
                     */


                    if (!idMaps[GetType()].ContainsSimilarKey(rawName)) //this Type has not been generated yet
                    {
                        idMaps[GetType()][rawNameKey] = 0;
                        id = rawName;
                    }
                    else
                    {
                        //The Id has been already generated for this Type with this rawName
                        //so just increase the index
                        var index = idMaps[GetType()][rawNameKey] + 1;

                        id = rawName + "." + index;
                        idMaps[GetType()][rawNameKey] = index;
                    }
                    //Trace.WriteLine(">>> " + GetType() + " >>> " + id);

                }
                return id;
            }
            set { id = value; }
        }
        /// <summary>
        /// Backing value of <see cref="Id"/>.
        /// </summary>
        protected string id;

        static Dictionary<Type, Dictionary<string, int>> idMaps = new Dictionary<Type, Dictionary<string, int>>();

        /// <summary>
        /// Resets the <see cref="Id"/> generator. This method is exercised by the Wix# compiler before any 
        /// <c>Build</c> operations to ensure reproducibility of the <see cref="Id"/> set between <c>Build()</c> 
        /// calls.
        /// </summary>
        static public void ResetIdGenerator()
        {
            idMaps.Clear();
        }

        internal bool IsIdSet()
        {
            return !id.IsEmpty();
        }
    }
}