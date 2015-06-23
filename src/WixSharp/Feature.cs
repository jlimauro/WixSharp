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

using System.Collections.Generic;
namespace WixSharp
{
    /// <summary>
    /// Defines WiX Feature. 
    /// <para>
    /// All installable WiX components belong to one or more features. By default, if no <see cref="Feature"/>s are defined by user, Wix# creates "Complete" 
    /// feature, which contains all installable components. 
    /// </para>
    /// </summary>
    /// <example>
    /// <list type="bullet">
    ///
    /// <item>
    /// <description>The example of defining <see cref="Feature"/>s explicitly:
    /// <code>
    /// Feature binaries = new Feature("My Product Binaries");
    /// Feature docs = new Feature("My Product Documentation");
    /// 
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(binaries, @"AppFiles\MyApp.exe",
    ///             ...
    /// </code>
    /// </description>
    /// </item>
    /// 
    /// <item>
    /// <description>The example of defining nested features .
    /// <code>
    /// Feature binaries = new Feature("My Product Binaries");
    /// Feature docs = new Feature("My Product Documentation");
    /// Feature docViewers = new Feature("Documentation viewrs");
    /// docs.Children.Add(docViewers);
    ///    ...
    /// </code>
    /// </description>
    /// </item>
    /// 
    /// <item>
    /// <description>The example of defining "Complete" <see cref="Feature"/> implicitly.
    /// Note <see cref="File"/> constructor does not use <see cref="Feature"/> argument.
    /// <code>
    /// var project =
    ///     new Project("My Product",
    ///         new Dir(@"%ProgramFiles%\My Company\My Product",
    ///             new File(@"AppFiles\MyApp.exe",
    ///             ...
    /// </code>
    /// </description>
    /// </item> 
    /// </list>
    /// </example>
    public partial class Feature : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class.
        /// </summary>
        public Feature()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        public Feature(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="description">The feature description.</param>
        public Feature(string name, string description)
            : this(name)
        {
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="isEnabled">Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.</param>
        public Feature(string name, bool isEnabled)
            : this(name)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="description">The feature description.</param>
        /// <param name="isEnabled">Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.</param>
        public Feature(string name, string description, bool isEnabled)
            : this(name)
        {
            Description = description;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="description">The feature description.</param>
        /// <param name="isEnabled">Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.</param>
        /// <param name="allowChange">Defines if setup allows the user interface to display an option to change the <see cref="Feature"/> state to Absent.</param>
        public Feature(string name, string description, bool isEnabled, bool allowChange)
            : this(name, description, isEnabled)
        {
            AllowChange = allowChange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="isEnabled">Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.</param>
        /// <param name="allowChange">Defines if setup allows the user interface to display an option to change the <see cref="Feature"/> state to Absent.</param>
        public Feature(string name, bool isEnabled, bool allowChange)
            : this(name, isEnabled)
        {
            AllowChange = allowChange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="description">The feature description.</param>
        /// <param name="configurableDir">The default path of the feature <c>ConfigurableDirectory</c>. If set to non-empty string, MSI runtime will place 
        /// <c>Configure</c> button for the feature in the <c>Feature Selection</c> dialog.</param>
        public Feature(string name, string description, string configurableDir)
            : this(name, description)
        {          
            ConfigurableDir = configurableDir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class  with properties/fields initialized with specified parameters.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="description">The feature description.</param>
        /// <param name="isEnabled">Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.</param>
        /// <param name="allowChange">Defines if setup allows the user interface to display an option to change the <see cref="Feature"/> state to Absent.</param>
        /// <param name="configurableDir">The default path of the feature <c>ConfigurableDirectory</c>. If set to non-empty string, MSI runtime will place 
        /// <c>Configure</c> button for the feature in the <c>Feature Selection</c> dialog.</param>
        public Feature(string name, string description, bool isEnabled, bool allowChange, string configurableDir)
            : this(name, description, isEnabled)
        {         
            AllowChange = allowChange;
            ConfigurableDir = configurableDir;
        }
        
        /// <summary>
        /// <para>
        /// Defines if the <see cref="Feature"/> is enabled at startup. 
        /// Use this parameter if the feature should be disabled by default and only enabled after 
        /// processing the <c>Condition Table</c> or user input.
        /// </para>
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IsEnabled = true;
        /// <summary>
        /// <para>
        /// Defines if setup allows the user interface to display an option to change the <see cref="Feature"/> state to Absent. 
        /// </para>
        /// <para>This property is translated into WiX Feature.Absent attribute.</para>
        /// The default value is <c>true</c>.
        /// </summary>
        public bool AllowChange = true;
        /// <summary>
        /// The feature description.
        /// </summary>
        public string Description = "";
        /// <summary>
        /// The default path of the feature <c>ConfigurableDirectory</c>. If set to non-empty string, MSI runtime will place 
        /// <c>Configure</c> button for the feature in the <c>Feature Selection</c> dialog.
        /// </summary>
        public string ConfigurableDir = "";

        internal Feature Parent;

        /// <summary>
        /// Child <see cref="Feature"/>. To be added in the nested Features scenarios.
        /// </summary>
        public List<Feature> Children = new List<Feature>();

        /// <summary>
        /// Defines the installation <see cref="Condition"/>, which is to be checked during the installation to 
        /// determine if the feature should be installed on the target system.
        /// </summary>
        public FeatureCondition Condition = null;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
