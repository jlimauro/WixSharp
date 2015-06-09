using System;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Represents a Wix Extension
    /// </summary>
    public class WixExtension
    {
        /// <summary>
        /// File name of the represented Wix Extension assembly
        /// </summary>
        /// <remarks>The represented value must include the file name and extension. See example</remarks>
        /// <example>WixIIsExtension.dll</example>
        public readonly string Assembly;
        /// <summary>
        /// Xml namespace declaration prefix for the represented Wix Extension
        /// </summary>
        public readonly string XmlNamespacePrefix;
        /// <summary>
        /// Xml namespace value for the represented Wix Extension
        /// </summary>
        public readonly string XmlNamespace;

        /// <summary>
        /// Creates a WixExtension instance representing the corresponding XML namespace declaration
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="prefix"></param>
        /// <param name="namespace"></param>
        public WixExtension(string assembly, string prefix, string @namespace)
        {
            if (assembly.IsNullOrEmpty()) throw new ArgumentNullException("assembly", "assembly is a null reference or empty");
            if (prefix.IsNullOrEmpty()) throw new ArgumentNullException("prefix", "prefix is a null reference or empty");
            if (@namespace.IsNullOrEmpty()) throw new ArgumentNullException("@namespace", "@namespace is a null reference or empty");

            Assembly = assembly;
            XmlNamespacePrefix = prefix;
            XmlNamespace = @namespace;
        }

        /// <summary>
        /// Returns XmlNamespacePrefix as an instance of XNamespace
        /// </summary>
        /// <returns></returns>
        public XNamespace ToXNamespace()
        {
            return XmlNamespace;
        }

        /// <summary>
        /// Gets the xml namespace attribute for this WixExtension
        /// </summary>
        /// <returns></returns>
        public string ToNamespaceDeclaration()
        {
            return GetNamespaceDeclaration(XmlNamespacePrefix, XmlNamespace);
        }

        /// <summary>
        /// Gets the xml namespace attribute for the provided <paramref name="prefix"/> and <paramref name="namespace"/>
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static string GetNamespaceDeclaration(string prefix, string @namespace)
        {
            return string.Format("xmlns:{0}=\"{1}\"", prefix, @namespace);
        }

        /// <summary>
        /// Well-known Wix Extension: Util
        /// </summary>
        public static WixExtension Util = new WixExtension("WixUtilExtension.dll", "util", "http://schemas.microsoft.com/wix/UtilExtension");

        /// <summary>
        /// Well-known Wix Extension IIs
        /// </summary>
        public static WixExtension IIs = new WixExtension("WixIIsExtension.dll", "iis", "http://schemas.microsoft.com/wix/IIsExtension");

        /// <summary>
        /// Well-known Wix Extension Sql
        /// </summary>
        public static WixExtension Sql = new WixExtension("WixSqlExtension.dll", "sql", "http://schemas.microsoft.com/wix/SqlExtension");

    }
}