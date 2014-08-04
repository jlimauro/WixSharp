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

using System.Xml.Linq;
namespace WixSharp
{
    /// <summary>
    /// Defines WiX <c>Condition</c>. <c>Condition</c> is normally associated with <c>CustomActions</c> or WiX elements (e.g. <c>Shortcut</c>).
    /// <para>
    /// 		<see cref="Condition"/> is nothing else but an XML friendly string wrapper, containing
    /// some predefined (commonly used) condition values. You can either use one of the
    /// predefined condition values (static members) or define your by specifying full string representation of
    /// the required WiX condition when calling the constructor or static method <c>Create</c>.
    /// </para>
    /// </summary>
    /// <example>The following is an example of initializing the Shortcut.<see cref="Shortcut.Condition"/>
    /// with custom value <c>INSTALLDESKTOPSHORTCUT="yes"</c> and
    /// the InstalledFileAction.<see cref="T:WixSharp.InstalledFileAction.Condition"/> with perefined value <c>NOT_Installed</c>:
    ///   <code>
    /// var project =
    ///     new Project("My Product",
    ///                 ...
    ///                 new Dir(@"%Desktop%",
    ///                     new WixSharp.Shortcut("MyApp", "[INSTALL_DIR]MyApp.exe", "")
    ///                     {
    ///                         Condition = new Condition("INSTALLDESKTOPSHORTCUT=\"yes\"")
    ///                     }),
    ///                 new InstalledFileAction("MyApp.exe", "",
    ///                                         Return.check,
    ///                                         When.After,
    ///                                         Step.InstallFinalize,
    ///                                         Condition.NOT_Installed),
    ///                                         ...
    ///   </code>
    ///   </example>
    public partial class Condition : WixEntity
    {
        /// <summary>
        /// String value of WiX <c>Condition</c>.
        /// </summary>
        public string Value = "";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="value">The value of the WiX condition expression.</param>
        public Condition(string value)
        {
            //Value = System.Security.SecurityElement.Escape(value);
            //Value = "<![CDATA[" + value + "]]>";
            Value = value;
        }

        /// <summary>
        ///  Returns the WiX <c>Condition</c> as a string.
        /// </summary>
        /// <returns>A string representing the condition.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Condition"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Condition obj)
        {
            return obj.ToString();
        }

        /// <summary>
        ///  Returns the WiX <c>Condition</c> as a <see cref="T:System.Xml.Linq.XCData"/>.
        ///  <remarks> Normally <c>Condition</c> is not designed to be parsed by the XML parser thus it should be embedded as CDATA</remarks>
        /// <code>
        /// &lt;Condition&gt;&lt;![CDATA[NETFRAMEWORK20="#0"]]&gt;&lt;/Condition&gt;
        /// </code> 
        /// </summary>
        /// <returns>A CDATA string representing the condition.</returns>
        public XCData ToCData()
        {
            return new XCData(Value);
        }
        /// <summary>
        /// String representation of the <c>NOT Installed</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_Installed = new Condition(" (NOT Installed) ");
        /// <summary>
        /// String representation of the <c>Installed</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition Installed = new Condition(" (Installed) ");
        /// <summary>
        /// String representation of the <c>NOT (REMOVE="ALL")</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_BeingRemoved = new Condition(" (NOT (REMOVE=\"ALL\")) ");
        /// <summary>
        /// String representation of the <c>UILevel > 3</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition NOT_Silent = new Condition(" (UILevel > 3) ");
        /// <summary>
        /// String representation of the <c>UILevel &lt; 4</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition Silent = new Condition(" UILevel < 4) ");
        /// <summary>
        /// String representation of the <c>REMOVE="ALL"</c> condition of the WiX <c>Condition</c>.
        /// </summary>
        public readonly static Condition BeingRemoved = new Condition(" (REMOVE=\"ALL\") ");
        /// <summary>
        /// Creates WiX <c>Condition</c> condition from the given string value.
        /// </summary>
        /// <param name="value">String value of the <c>Condition</c> to be created.</param>
        /// <returns>Instance of the <c>Condition</c></returns>
        /// <example>The following is an example of initializing the Shortcut.<see cref="Shortcut.Condition"/>  
        /// with custom value <c>INSTALLDESKTOPSHORTCUT="yes"</c>:
        /// <code>
        ///     new Dir(@"%Desktop%",
        ///         new WixSharp.Shortcut("MyApp", "[INSTALL_DIR]MyApp.exe", "")
        ///         {
        ///            Condition = Condition.Create("INSTALLDESKTOPSHORTCUT=\"yes\"") 
        ///         })
        ///         
        /// </code>
        /// </example>
        public static Condition Create(string value) { return new Condition(value); }

        //public Condition And(Condition condition)
        //{
        //    return Create("(" + this.ToString() + " AND " + condition.ToString() + ")");
        //}

        //public Condition Or(Condition condition)
        //{
        //    return Create("(" + this.ToString() + " OR " + condition.ToString() + ")");
        //}

        //public Condition Invert()
        //{
        //    return Create("NOT (" + this.ToString() + ")");
        //}
    }
}
