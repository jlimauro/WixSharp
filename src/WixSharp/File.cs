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
using System.Xml.Linq;

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
        /// <para>This property is designed to produce a friendlier string representation of the <see cref="File"/>
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
            Shortcuts = items.OfType<FileShortcut>().ToArray();
            Associations = items.OfType<FileAssociation>().ToArray();
            IISVirtualDirs = items.OfType<IISVirtualDir>().ToArray();
            ServiceInstaller = items.OfType<ServiceInstaller>().FirstOrDefault();
            Permissions = items.OfType<FilePermission>().ToArray();

            var firstUnExpectedItem = items.Except(Shortcuts)
                                           .Except(Associations)
                                           .Except(IISVirtualDirs)
                                           .Except(Permissions)
                                           .Where(x => x != ServiceInstaller)
                                           .ToArray();

            if (firstUnExpectedItem.Any())
                throw new ApplicationException("{4} is unexpected. Only {0}, {1}, {2}, and {3} items can be added to {4}".FormatInline(
                                                                                                       typeof(FileShortcut),
                                                                                                       typeof(FileAssociation),
                                                                                                       typeof(ServiceInstaller),
                                                                                                       typeof(FilePermission),
                                                                                                       this.GetType(),
                                                                                                       firstUnExpectedItem.First().GetType()));
        }

        /// <summary>
        /// Collection of the <see cref="FileAssociation"/>s associated with the file. 
        /// </summary>
        public FileAssociation[] Associations = new FileAssociation[0];

        /// <summary>
        /// The service installer associated with the file. Set this field to the properly initialized
        /// instance of <see cref="ServiceInstaller"/> if the file is a windows service module.
        /// </summary>
        public ServiceInstaller ServiceInstaller = null;

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
        /// <summary>
        /// COllection of <see cref="FilePermsission"/> to be applied to the file. 
        /// </summary>
        public FilePermission[] Permissions = new FilePermission[0];
        /// <summary>
        /// Gets or sets the NeverOverwrite attribute of the associated WiX component.
        /// <para>If this attribute is set to 'true', the installer does not install or reinstall the component
        ///  if a key path file for the component already exists. </para>
        /// </summary>
        /// <value>
        /// The never overwrite.
        /// </value>
        public bool? NeverOverwrite
        {
            get
            {
                var value = GetAttributeDefinition("Component:NeverOverwrite");
                if (value == null)
                    return null;
                else
                   return (value == "yes");
            }
            set
            {
                if (value.HasValue)
                    SetAttributeDefinition("Component:NeverOverwrite", value.Value.ToYesNo());
                else
                    SetAttributeDefinition("Component:NeverOverwrite", null);
            }
        }
    }

    /// <summary>
    /// Enumeration representing Generic* attributes of PermissionEx element
    /// </summary>
    [Flags]
    public enum GenericPermission
    {
        /// <summary>
        /// None does not map to a valid WiX representation
        /// </summary>
        None = 0,
        /// <summary>
        /// Maps to GenericExecute='yes' of PermissionEx
        /// </summary>
        Execute = 0x001, 
        /// <summary>
        /// Maps to GenericWrite='yes' of PermissionEx
        /// </summary>
        Write = 0x010,
        /// <summary>
        /// Maps to GenericRead='yes' of PermissionEx
        /// </summary>
        Read = 0x100,
        /// <summary>
        /// Maps to GenericAll='yes' of PermissionEx
        /// </summary>
        All = Execute | Write | Read
    }

    /// <summary>
    /// Represents applying permission(s) to the containing File entity
    /// </summary>
    public class FilePermission : WixEntity
    {

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>
        /// </summary>
        /// <param name="user"></param>
        public FilePermission(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");
            User = user;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        public FilePermission(string user, string domain)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/> with generic permissions described by <paramref name="permission"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        public FilePermission(string user, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;

            SetGenericPermission(permission);
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/> with generic permissions described by <paramref name="permission"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <param name="permission"></param>
        public FilePermission(string user, string domain, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;

            SetGenericPermission(permission);
        }

        private void SetGenericPermission(GenericPermission permission)
        {
            if (permission == GenericPermission.All)
            {
                GenericAll = true;
                return;
            }

            if ((permission & GenericPermission.Execute) == GenericPermission.Execute)
                GenericExecute = true;

            if ((permission & GenericPermission.Write) == GenericPermission.Write)
                GenericWrite = true;

            if ((permission & GenericPermission.Read) == GenericPermission.Read)
                GenericRead = true;
        }

        /// <summary>
        /// Maps to the User property of PermissionEx
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// Maps to the Domain property of PermissionEx
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Maps to the Append property of PermissionEx
        /// </summary>
        public bool? Append { get; set; }

        /// <summary>
        /// Maps to the ChangePermission property of PermissionEx
        /// </summary>
        public bool? ChangePermission { get; set; }

        /// <summary>
        /// Maps to the CreateLink property of PermissionEx
        /// </summary>
        public bool? CreateLink { get; set; }

        /// <summary>
        /// Maps to the CreateSubkeys property of PermissionEx
        /// </summary>
        public bool? CreateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Delete property of PermissionEx
        /// </summary>
        public bool? Delete { get; set; }

        /// <summary>
        /// Maps to the EnumerateSubkeys property of PermissionEx
        /// </summary>
        public bool? EnumerateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Execute property of PermissionEx
        /// </summary>
        public bool? Execute {get; set; }

        /// <summary>
        /// Maps to the GenericAll property of PermissionEx
        /// </summary>
        public bool? GenericAll { get; set; }

        /// <summary>
        /// Maps to the GenericExecute property of PermissionEx
        /// </summary>
        public bool? GenericExecute { get; set; }

        /// <summary>
        /// Maps to the GenericRead property of PermissionEx
        /// </summary>
        public bool? GenericRead {get; set; }

        /// <summary>
        /// Maps to the GenericWrite property of PermissionEx
        /// </summary>
        public bool? GenericWrite { get; set; }

        /// <summary>
        /// Maps to the Notify property of PermissionEx
        /// </summary>
        public bool? Notify { get; set; }

        /// <summary>
        /// Maps to the Read property of PermissionEx
        /// </summary>
        public bool? Read { get; set; }

        /// <summary>
        /// Maps to the Readattributes property of PermissionEx
        /// </summary>
        public bool? Readattributes { get; set; }

        /// <summary>
        /// Maps to the ReadExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? ReadExtendedAttributes { get; set; }

        /// <summary>
        /// Maps to the ReadPermission property of PermissionEx
        /// </summary>
        public bool? ReadPermission { get; set; }

        /// <summary>
        /// Maps to the Synchronize property of PermissionEx
        /// </summary>
        public bool? Synchronize { get; set; }

        /// <summary>
        /// Maps to the TakeOwnership property of PermissionEx
        /// </summary>
        public bool? TakeOwnership { get; set; }

        /// <summary>
        /// Maps to the Write property of PermissionEx
        /// </summary>
        public bool? Write { get; set; }

        /// <summary>
        /// Maps to the WriteAttributes property of PermissionEx
        /// </summary>
        public bool? WriteAttributes { get; set; }

        /// <summary>
        /// Maps to the WriteExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? WriteExtendedAttributes { get; set; }

    }

    internal static class FilePermissionExt
    {

        static void Do<T>(this T? nullable, Action<T> action) where T : struct
        {
            if (!nullable.HasValue) return;
            action(nullable.Value);
        }

        public static void EmitAttributes(this FilePermission filePermission, XElement permissionElement)
        {
            //required
            permissionElement.SetAttributeValue("User", filePermission.User);
            //optional
            if (filePermission.Domain.IsNotEmpty()) permissionElement.SetAttributeValue("Domain", filePermission.Domain);

            //optional
            filePermission.Append.Do(b => permissionElement.SetAttributeValue("Append", b.ToYesNo()));
            filePermission.ChangePermission.Do(b => permissionElement.SetAttributeValue("ChangePermission", b.ToYesNo()));
            filePermission.CreateLink.Do(b => permissionElement.SetAttributeValue("CreateLink", b.ToYesNo()));
            filePermission.CreateSubkeys.Do(b => permissionElement.SetAttributeValue("CreateSubkeys", b.ToYesNo()));
            filePermission.Delete.Do(b => permissionElement.SetAttributeValue("Delete", b.ToYesNo()));
            filePermission.EnumerateSubkeys.Do(b => permissionElement.SetAttributeValue("EnumerateSubkeys", b.ToYesNo()));
            filePermission.Execute.Do(b => permissionElement.SetAttributeValue("Execute", b.ToYesNo()));
            filePermission.GenericAll.Do(b => permissionElement.SetAttributeValue("GenericAll", b.ToYesNo()));
            filePermission.GenericExecute.Do(b => permissionElement.SetAttributeValue("GenericExecute", b.ToYesNo()));
            filePermission.GenericRead.Do(b => permissionElement.SetAttributeValue("GenericRead", b.ToYesNo()));
            filePermission.GenericWrite.Do(b => permissionElement.SetAttributeValue("GenericWrite", b.ToYesNo()));
            filePermission.Notify.Do(b => permissionElement.SetAttributeValue("Notify", b.ToYesNo()));
            filePermission.Read.Do(b => permissionElement.SetAttributeValue("Read", b.ToYesNo()));
            filePermission.Readattributes.Do(b => permissionElement.SetAttributeValue("Readattributes", b.ToYesNo()));
            filePermission.ReadExtendedAttributes.Do(b => permissionElement.SetAttributeValue("ReadExtendedAttributes", b.ToYesNo()));
            filePermission.ReadPermission.Do(b => permissionElement.SetAttributeValue("ReadPermission", b.ToYesNo()));
            filePermission.Synchronize.Do(b => permissionElement.SetAttributeValue("Synchronize", b.ToYesNo()));
            filePermission.TakeOwnership.Do(b => permissionElement.SetAttributeValue("TakeOwnership", b.ToYesNo()));
            filePermission.Write.Do(b => permissionElement.SetAttributeValue("Write", b.ToYesNo()));
            filePermission.WriteAttributes.Do(b => permissionElement.SetAttributeValue("WriteAttributes", b.ToYesNo()));
            filePermission.WriteExtendedAttributes.Do(b => permissionElement.SetAttributeValue("WriteExtendedAttributes", b.ToYesNo()));
        }
    }

}
