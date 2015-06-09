using System;
using System.Xml.Linq;

namespace WixSharp
{

    /// <summary>
    /// Represents a WixUtilExtension User
    /// </summary>
    public class User : WixEntity
    {

        #region Constructors

        public User() { }

        public User(Id id, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "name is a null reference or empty");

            Name = name;
            Id = id;
        }

        public User(Id id, Feature feature, string name)
            : this(id, name)
        {
            Id = id;
            Feature = feature;
        }

        public User(Id id, string name, string domain)
            : this(id, name)
        {
            Domain = domain;
        }

        public User(Id id, Feature feature, string name, string domain)
            : this(id, feature, name)
        {
            Domain = domain;
        }

        #endregion

        public Feature Feature { get; set; }

        /// <summary>
        /// Requests that the User element is included inside a Component element - allowing the User to be installed.
        /// If any of the following properties are assigned (non-null), this property is ignored during compilation and assumed
        /// to be true:
        /// <list type="bullet">
        /// <item>CanNotChangePassword</item>
        /// <item>CreateUser</item>
        /// <item>Disabled</item>
        /// <item>FailIfExists</item>
        /// <item>LogOnAsBatchJob</item>
        /// <item>LogOnAsService</item>
        /// <item>PasswordExpired</item>
        /// <item>PasswordNeverExpires</item>
        /// <item>RemoveOnUninstall</item>
        /// <item>UpdateIfExists</item>
        /// <item>Vital</item>
        /// </list>
        /// </summary>
        public bool WixIncludeInComponent { get; set; }

        #region Wix User attributes

        public bool? CanNotChangePassword { get; set; } //only allowed under a component
        public bool? CreateUser { get; set; } //only allowed under a component
        public bool? Disabled { get; set; } //only allowed under a component
        public string Domain { get; set; }
        public bool? FailIfExists { get; set; } //only allowed under a component
        public bool? LogOnAsBatchJob { get; set; } //only allowed under a component
        public bool? LogOnAsService { get; set; } //only allowed under a component
        public string Name { get; set; } //required
        public string Password { get; set; }
        public bool? PasswordExpired { get; set; } //only allowed under a component
        public bool? PasswordNeverExpires { get; set; } //only allowed under a component
        public bool? RemoveOnUninstall { get; set; } //only allowed under a component
        public bool? UpdateIfExists { get; set; } //only allowed under a component
        public bool? Vital { get; set; } //only allowed under a component

        #endregion

        /// <summary>
        /// Gets a value indicated if this User must be generated under a Component element or not.
        /// </summary>
        internal bool MustDescendFromComponent
        {
            get
            {
                return CanNotChangePassword.HasValue
                       || CreateUser.HasValue
                       || Disabled.HasValue
                       || FailIfExists.HasValue
                       || LogOnAsBatchJob.HasValue
                       || LogOnAsService.HasValue
                       || PasswordExpired.HasValue
                       || PasswordNeverExpires.HasValue
                       || RemoveOnUninstall.HasValue
                       || UpdateIfExists.HasValue
                       || Vital.HasValue
                       || WixIncludeInComponent;
            }
        }

    }

    internal static class UserExt
    {
        static void Do<T>(this T? nullable, Action<T> action) where T : struct
        {
            if (!nullable.HasValue) return;
            action(nullable.Value);
        }

        public static void EmitAttributes(this User user, XElement userElement)
        {
            userElement.SetAttributeValue("Id", user.Id);
            userElement.SetAttributeValue("Name", user.Name);

            user.CanNotChangePassword.Do(b => userElement.SetAttribute("CanNotChangePassword", b.ToYesNo()));
            user.CreateUser.Do(b => userElement.SetAttribute("CreateUser", b.ToYesNo()));
            user.Disabled.Do(b => userElement.SetAttribute("Disabled", b.ToYesNo()));
            if (!string.IsNullOrEmpty(user.Domain)) userElement.SetAttributeValue("Domain", user.Domain);
            user.FailIfExists.Do(b => userElement.SetAttribute("FailIfExists", b.ToYesNo()));
            user.LogOnAsBatchJob.Do(b => userElement.SetAttribute("LogOnAsBatchJob", b.ToYesNo()));
            user.LogOnAsService.Do(b => userElement.SetAttribute("LogOnAsService", b.ToYesNo()));
            if (!string.IsNullOrEmpty(user.Password)) userElement.SetAttributeValue("Password", user.Password);
            user.PasswordExpired.Do(b => userElement.SetAttribute("PasswordExpired", b.ToYesNo()));
            user.PasswordNeverExpires.Do(b => userElement.SetAttribute("PasswordNeverExpires", b.ToYesNo()));
            user.RemoveOnUninstall.Do(b => userElement.SetAttribute("RemoveOnUninstall", b.ToYesNo()));
            user.UpdateIfExists.Do(b => userElement.SetAttribute("UpdateIfExists", b.ToYesNo()));
            user.Vital.Do(b => userElement.SetAttribute("Vital", b.ToYesNo()));
        }
    }
}
