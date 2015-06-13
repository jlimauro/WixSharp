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
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using IO = System.IO;

namespace WixSharp
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="element">Element to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, XElement element)
        {
            obj.Add(element);
            return element;
        }

        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// <para>
        /// <c>elementName</c> can be either the name of the element to be added or the sequence of the elements specified by path (e.g. <c>AddElement("Product/Package")</c>).
        /// </para>
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="elementName">Element to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, string elementName)
        {
            var parent = obj;
            foreach (var item in elementName.Split('/'))
                parent = parent.AddElement(new XElement(item));
            return parent;
        }

        /// <summary>
        /// Adds the element to a given XML element and sets the attributes of the newly created element.
        /// <para>
        /// <c>elementName</c> can be either the name of the element to be added or the sequence of the elements specified by path (e.g. <c>AddElement("Product/Package")</c>).
        /// </para>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="elementName">The element.</param>
        /// <param name="attributesDefinition">The attributes definition. Rules of the composing the
        /// definition are the same as for <see cref="WixEntity.AttributesDefinition"/>.</param>
        /// <returns></returns>
        public static XElement AddElement(this XElement obj, string elementName, string attributesDefinition)
        {
            return obj.AddElement(elementName).AddAttributes(attributesDefinition);
        }

        /// <summary>
        /// Converts key/value map into the dictionary. The map entry format
        /// is as follows: &lt;key&gt;=&lt;value&gt;[;&lt;key&gt;=&lt;value&gt;].
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="itemDelimiter">The item delimiter.</param>
        /// <param name="valueDelimiter">The value delimiter.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid map entry</exception>
        public static Dictionary<string, string> ToDictionary(this string map, char itemDelimiter = ';', char valueDelimiter = '=')
        {
            var retval = new Dictionary<string, string>();
            if (!map.IsEmpty())
            {
                foreach (string pair in map.Trim().Split(new[] { itemDelimiter }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (pair.IsNotEmpty())
                        try
                        {
                            string[] tokens = pair.Split(new[] { valueDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                            string name = tokens[0].Trim();

                            string value = "";
                            if (tokens.Count() > 1)
                                value = tokens[1].Trim();

                            retval[name] = value;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Invalid map entry", e);
                        }
                }

            }
            return retval;
        }
        /// <summary>
        /// Adds the element to a given XML element. It is a Fluent version of <see cref="T:System.Xml.Linq.XElement.Add"/>.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="element">Element to add.</param>
        /// <param name="attributes">The collection of Name/Value attributes to add.</param>
        /// <returns>Added <see cref="T:System.Xml.Linq.XElement"/>.</returns>
        public static XElement AddElement(this XElement obj, XElement element, Dictionary<string, string> attributes)
        {
            obj.Add(element.AddAttributes(attributes));
            return element;
        }

        /// <summary>
        /// Adds the attributes to the to a given XML element (<see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="attributesDefinition">The attributes definition. Rules of the composing the
        /// definition are the same as for <see cref="WixEntity.AttributesDefinition"/>.</param>
        /// <returns></returns>
        public static XElement AddAttributes(this XElement obj, string attributesDefinition)
        {
            return obj.AddAttributes(attributesDefinition.ToDictionary());
        }

        /// <summary>
        /// Sets the value of the attribute. This is a fluent version of XElement.SetAttributeValue. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, string name, object value)
        {
            if (value is string && (value as string).IsEmpty())
                obj.SetAttributeValue(name, null);
            else
                obj.SetAttributeValue(name, value);
            return obj;
        }

        /// <summary>
        /// Sets the value of the attribute. This is a fluent version of XElement.SetAttributeValue that takes the Name/Value 
        /// string definition as a single input parameter.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="nameValuePair">The attribute name/value pair of the "[name]=[value]" format (e.g. ""Version=!(bind.FileVersion.Utils.dll)").</param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, string nameValuePair)
        {
            var pair = nameValuePair.ToDictionary().FirstOrDefault();

            if (pair.Value is string && pair.Value.IsEmpty())
                obj.SetAttributeValue(pair.Key, null);
            else
                obj.SetAttributeValue(pair.Key, pair.Value);
            return obj;
        }

        /// <summary>
        /// Adds the attributes to the to a given XML element (<see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="attributes">The collection of Name/Value attributes to add.</param>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> with added attributes.</returns>
        public static XElement AddAttributes(this XElement obj, Dictionary<string, string> attributes)
        {
            if (attributes.Any())
            {
                var optimizedAttributes = attributes.Where(x => !x.Key.Contains(":")).ToDictionary(t => t.Key, t => t.Value);

                var compositValues = string.Join(";", attributes.Where(x => x.Key.Contains(":")).Select(x => x.Key + "=" + x.Value).ToArray());
                if (compositValues.IsNotEmpty())
                    optimizedAttributes.Add("WixSharpCustomAttributes", compositValues);

                foreach (var key in optimizedAttributes.Keys)
                    obj.SetAttributeValue(key, optimizedAttributes[key]);
            }
            return obj;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement obj, string name)
        {
            return obj.Attribute(name) != null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute and the attribute value passes the test
        /// by <c>attributeValueSelector</c>.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <param name="attributeValueSelector">The attribute value selector. Allows testing the attribute value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="T:System.Xml.Linq.XElement"/> has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute(this XElement obj, string name, Predicate<string> attributeValueSelector)
        {
            return obj.Attribute(name) != null && attributeValueSelector(obj.Attribute(name).Value);
        }

        /// <summary>
        /// Search for the first parent element (in the "parents chain") with the specified name of the given XML element (<see cref="T:System.Xml.Linq.XElement"/>).
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Xml.Linq.XElement"/>.</param>
        /// <param name="parentName">Name of the parent element to search.</param>
        /// <returns><see cref="T:System.Xml.Linq.XElement"/> with the matching name.</returns>
        public static XElement Parent(this XElement obj, string parentName)
        {
            XElement element = obj.Parent;
            do
            {
                if (element.Name.LocalName == parentName)
                    return element;
                else
                    element = element.Parent;
            }
            while (element != null);

            return null;
        }
        /// <summary>
        /// Copies attribute value from one <see cref="T:System.Xml.Linq.XElement"/> to another. If the attribute already exists, its value is modified.
        /// </summary>
        /// <param name="dest">The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.</param>
        /// <param name="src">The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute from.</param>
        /// <param name="attributeName">Name of the source attribute to copy.</param>
        /// <returns>The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.</returns>
        public static XElement CopyAttributeFrom(this XElement dest, XElement src, string attributeName)
        {
            if (src.Attribute(attributeName) != null)
            {
                if (dest.Attribute(attributeName) != null)
                    dest.Attribute(attributeName).Value = src.Attribute(attributeName).Value;
                else
                    dest.Add(new XAttribute(attributeName, src.Attribute(attributeName).Value));
            }
            return dest;
        }

        /// <summary>
        /// Reads the attribute value. Returns null if attribute doesn't exist.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string ReadAttribute(this XElement e, string attributeName)
        {
            if (e.Attribute(attributeName) != null)
                return e.Attribute(attributeName).Value;
            else
                return null;
        }

        /// <summary>
        /// A generic LINQ equivalent of C# foreach loop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
            return collection;
        }
        /// <summary>
        /// Gets the combined hash code of all items in the collection. This method is convenient to use to 
        /// verify that the collections have identical items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static int GetItemsHashCode<T>(this IEnumerable<T> collection)
        {
            var hash = new StringBuilder();
            foreach (T item in collection)
            {
                hash.Append((item == null ? "null".GetHashCode() : item.GetHashCode()).ToString());
            }
            return hash.ToString().GetHashCode();
        }
        /// <summary>
        /// Copies attribute value from one <see cref="T:System.Xml.Linq.XElement"/> to another. If the attribute already exists, its value is modified.
        /// </summary>
        /// <param name="dest">The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.</param>
        /// <param name="destAttributeName">Name of the destination attribute to copy.</param>
        /// <param name="src">The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute from.</param>
        /// <param name="srcAttributeName">Name of the source attribute to copy.</param>
        /// <returns>The instance of the <see cref="T:System.Xml.Linq.XElement"/> to copy the attribute to.</returns>
        public static XElement CopyAttributeFrom(this XElement dest, string destAttributeName, XElement src, string srcAttributeName)
        {
            if (src.Attribute(srcAttributeName) != null)
            {
                if (dest.Attribute(destAttributeName) != null)
                    dest.Attribute(destAttributeName).Value = src.Attribute(srcAttributeName).Value;
                else
                    dest.Add(new XAttribute(destAttributeName, src.Attribute(srcAttributeName).Value));
            }
            return dest;
        }
        /// <summary>
        /// Replaces all Wix# predefined string constants in the Wix# directory path with their WiX equivalents. 
        /// <para>Processed string can be used as an Id for referencing from other Wix# components and setting the 
        /// corresponding path from <c>MsiExec.exe</c> command line.</para>
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <returns>Replacement/conversion result.</returns>
        public static string ToDirID(this string path)
        {
            return path.Expand();
        }

        /// <summary>
        /// Safely converts string to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int ToInt(this string value, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// Safely converts string to IntPtr.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IntPtr ToIntPtr(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return (IntPtr)result;
        }

        /// <summary>
        /// Simple wrapper around System.String.Compare(string strA, string strB, bool ignoreCase);
        /// </summary>
        /// <param name="strA">The string a.</param>
        /// <param name="strB">The string b.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static bool SameAs(this string strA, string strB, bool ignoreCase = false)
        {
            return 0 == string.Compare(strA, strB, ignoreCase);
        }

        /// <summary>
        /// Returns true if bothe values represent the same path.
        /// </summary>
        /// <param name="pathA">The path a.</param>
        /// <param name="pathB">The path b.</param>
        /// <returns></returns>
        public static bool SamePathAs(this string pathA, string pathB)
        {
            return 0 == string.Compare(IO.Path.GetFullPath(pathA), IO.Path.GetFullPath(pathB), true);
        }

        /// <summary>
        /// The change directory of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="newDir">The new dir.</param>
        /// <returns></returns>
        public static string PathChangeDirectory(this string path, string newDir)
        {
            return IO.Path.Combine(newDir, IO.Path.GetFileName(path));
        }

        /// <summary>
        /// Change extension of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string PathChangeExtension(this string path, string extension)
        {
            return IO.Path.ChangeExtension(path, extension);
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string PathGetFullPath(this string path)
        {
            return IO.Path.GetFullPath(path);
        }

        /// <summary>
        /// Formats the specified string.
        /// </summary>
        /// <param name="obj">The string to format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatInline(this string obj, params object[] args)
        {
            return string.Format(obj, args);
        }

        /// <summary>
        /// Splits string by lines. The method handles both '\r\n' and '\n' line endings.
        /// </summary>
        /// <param name="text">The text to be split.</param>
        /// <returns></returns>
        public static string[] GetLines(this string text)
        {
            return text.Replace("\r\n", "\n").Split('\n');
        }

        /// <summary>
        /// Replaces all Wix# predefined string constants (Environment Constants) in the Wix# directory path with their WiX equivalents and escapes all WiX illegal characters (e.g. space character). 
        /// <para>
        /// <para>It also replaces all "illegal" characters (e.g. !,\) with '_' character to allow the path value to be used as a WiX Id XML attribute.</para>
        /// <example>The following is an example of expanding directory name paths.
        /// <code>
        /// @"%ProgramFiles%\My Company\My Product".Expand()       -> @"ProgramFilesFolder\My_Company\My_Product"
        /// @"ProgramFilesFolder\My Company\My Product".Expand()   -> @"ProgramFilesFolder\My_Company\My_Product"
        /// @"[ProgramFilesFolder]\My Company\My Product".Expand() -> @"ProgramFilesFolder\My_Company\My_Product"
        /// </code>
        /// </example>
        /// </para>
        /// For the list of supported constants analyses <c>WixSharp.Compiler.EnvironmentConstantsMapping.Keys</c>.
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <returns>Replacement result.</returns>
        public static string Expand(this string path)
        {
            //directory ID (e.g. %ProgramFiles%\My Company\My Product should be interpreted as ProgramFilesFolder\My Company\My Product)
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, Compiler.EnvironmentConstantsMapping[key])
                           .Replace("[" + Compiler.EnvironmentConstantsMapping[key] + "]", Compiler.EnvironmentConstantsMapping[key]);

            return path.ExpandWixEnvConsts()
                       .Replace("\\", ".")
                       .EscapeIllegalCharacters();
        }

        /// <summary>
        /// Determines whether the string contains WiX constants (values enclosed into square brackets).
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static bool ContainsWixConstants(this string data)
        {
            return data.Contains("[") || data.Contains("]"); 
        }

        /// <summary>
        /// Maps the Wix# constants included in path into their x64 equivalents.
        /// <para>For example %ProgramFiles%\My Company\My Product should be preprocessed into %ProgramFiles64%\My Company\My Product</para>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string Map64Dirs(this string path)
        {
            //directory ID (e.g. %ProgramFiles%\My Company\My Product should be preprocessed into %ProgramFiles64%\My Company\My Product)
            foreach (string key in Compiler.EnvironmentFolders64Mapping.Keys)
            {
                if (path.Contains(key))
                    path = path.Replace(key, Compiler.EnvironmentFolders64Mapping[key]);
            }
            return path;
        }

        /// <summary>
        /// Replaces all Wix# predefined string constants (Environment Constants) in the Wix# directory path with their WiX equivalents and escapes all WiX illegal characters (e.g. space character). 
        /// <para>
        /// <example>The following is an example of expanding directory name paths.
        /// <code>
        /// @"%ProgramFiles%\My Company\My Product".Expand()       -> @"ProgramFilesFolder\My_Company\My_Product"
        /// @"ProgramFilesFolder\My Company\My Product".Expand()   -> @"ProgramFilesFolder\My_Company\My_Product"
        /// @"[ProgramFilesFolder]\My Company\My Product".Expand() -> @"ProgramFilesFolder\My_Company\My_Product"
        /// </code>
        /// </example>
        /// </para>
        /// For the list of supported constants analyse <c>WixSharp.Compiler.EnvironmentConstantsMapping.Keys</c>.
        /// </summary>
        /// <param name="path">The Wix# directory path.</param>
        /// <returns>Replacement result.</returns>
        public static string ExpandWixEnvConsts(this string path)
        {
            //directory ID (e.g. %ProgramFiles%\My Company\My Product should be interpreted as ProgramFilesFolder\My Company\My Product)
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, Compiler.EnvironmentConstantsMapping[key])
                           .Replace("[" + Compiler.EnvironmentConstantsMapping[key] + "]", Compiler.EnvironmentConstantsMapping[key]);
            return path;
        }

        /// <summary>
        /// Expands the EnvironmentVariable It is nothing else but a an extension method wrapping Environment.ExpandEnvironmentVariables to allow fluent API.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string ExpandEnvVars(this string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
        }

        internal static string EscapeIllegalCharacters(this string data)
        {
            string retval = data;
            List<char> legalChars = new List<char>();

            legalChars.AddRange("._0123456789".ToCharArray());
            legalChars.AddRange("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());

            for (int i = 0; i < data.Length; i++)
            {
                if (!legalChars.Contains(retval[i]))
                    retval = retval.Replace(data[i], '_');
            }
            return retval;
        }

        internal static string ExpandCommandPath(this string path)
        {
            foreach (string key in Compiler.EnvironmentConstantsMapping.Keys)
                path = path.Replace(key, "[" + Compiler.EnvironmentConstantsMapping[key] + "]");

            return path;
        }

        internal static bool ContainsSimilarKey<T>(this Dictionary<string, T> dictionary, string value)
        {
            foreach (string key in dictionary.Keys)
                if (string.Compare(key, value, true) == 0)
                    return true;
            return false;
        }

        internal static string ToYesNo(this bool obj)
        {
            return obj ? "yes" : "no";
        }
        /// <summary>
        /// Determines whether the given string is empty.
        /// </summary>
        /// <param name="s">The string to analyse.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        /// <summary>
        /// Determines whether the given string is empty or not.
        /// </summary>
        /// <param name="s">The string to analyse.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is not empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
        /// <summary>
        /// Returns all leading white-space characters.
        /// </summary>
        /// <param name="s">The string to analyse.</param>
        /// <returns>
        /// 	Total count of leading white-space characters
        /// </returns>
        public static int GetLeftIndent(this string s)
        {
            return s.Length - s.TrimStart('\n', '\r', '\t', ' ').Length;
        }


        /// <summary>
        /// Concats the specified strings. In the result string all items are separated with the specified delimiter.
        /// </summary>
        /// <param name="strings">The strings.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string ConcatItems(this IEnumerable<string> strings, string delimiter)
        {
            StringBuilder retval = new StringBuilder();
            foreach (var s in strings)
            {
                retval.Append(s);
                retval.Append(delimiter);
            }
            return retval.ToString();
        }
        /// <summary>
        /// Selects from the given element the first child element matching the specified path (e.g. <c>Select("Product/Package")</c>).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement Select(this XContainer element, string path)
        {
            string[] parts = path.Split('/');

            var ttt = element.Elements().ToArray();

            var e = (from el in element.Elements()
                     where el.Name.LocalName == parts[0]
                     select el).GetEnumerator();

            if (!e.MoveNext())
                return null;

            if (parts.Length == 1) //the last link in the chain
                return e.Current;
            else
                return e.Current.Select(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        /// <summary>
        /// Selects from the given element the first child element Directory matching the specified path (e.g. <c>Select("ProgramFiles/MyCompany") by </c>).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement FindDirectory(this XElement element, string path)
        {
            string[] parts = path.Split('/');

            var e = (from el in element.Elements()
                     where el.Name.LocalName == "Directory" && el.Attribute("Name") != null && el.Attribute("Name").Value == parts[0]
                     select el).GetEnumerator();

            if (!e.MoveNext())
                return null;

            if (parts.Length == 1) //the last link in the chain
                return e.Current;
            else
                return e.Current.FindDirectory(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        /// <summary>
        /// Flattened "view" of all elements with a given name (LocalName).
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="elementName">The element name.</param>
        /// <returns>The elements matching the name.</returns>
        [Obsolete("AllElements is obsolete. Please use more efficient FindAll instead.")]
        public static XElement[] AllElements(this XElement element, string elementName)
        {
            int iterator = 0;
            var elementsList = new List<XElement>();
            var matchingElementList = new List<XElement>();

            elementsList.Add(element);

            while (iterator < elementsList.Count)
            {
                foreach (XElement e in elementsList[iterator].Elements())
                {
                    elementsList.Add(e);

                    if (e.Name.LocalName == elementName)
                        matchingElementList.Add(e);
                }

                iterator++;
            }

            return matchingElementList.ToArray();
        }


        public static bool HasLocalName(this XElement element, string elementName, bool ignoreCase = false)
        {
            return element.Name.LocalName.SameAs(elementName, ignoreCase);
        }

        internal static void Map(this Dictionary<Feature, List<string>> featureComponents, Feature feature, string componentId)
        {
            if (!featureComponents.ContainsKey(feature))
                featureComponents[feature] = new List<string>();

            featureComponents[feature].Add(componentId);
        }


        /// <summary>
        /// Selects single descendant element with a given name (LocalName). Throws if no or more then one match found
        /// </summary>
        /// <param name="container">The element to be searched.</param>
        /// <param name="elementName">The element local name.</param>
        /// <returns>The elements matching the name.</returns>
        public static XElement FindSingle(this XContainer container, string elementName)
        {
            return container.Descendants().Single(x => x.Name.LocalName == elementName);
        }

        /// <summary>
        /// Selects all descendant elements with a given name (LocalName). Throws if no or more then one match found
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="elementName">The element local name.</param>
        /// <returns>The elements matching the name.</returns>
        public static XElement[] FindAll(this XContainer element, string elementName)
        {
            return element.Descendants().Where(x => x.Name.LocalName == elementName).ToArray();
        }

        /// <summary>
        /// Removes the element from its current parent and inserts it into another element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="newParent">The new parent.</param>
        /// <returns></returns>
        public static XElement MoveTo(this XElement element, XElement newParent)
        {
            element.Remove();
            newParent.Add(element);
            return element;
        }
       
        /// <summary>
        /// Selects, from the given element, the child element matching the specified path.
        /// <para>If the child element is not found, a new element is created matching the path.</para>
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="path">The path.</param>
        /// <returns>The element matching the path.</returns>
        public static XElement SelectOrCreate(this XElement element, string path)
        {
            string[] parts = path.Split('/');

            var e = (from el in element.Elements()
                     where el.Name.LocalName == parts[0]
                     select el).GetEnumerator();

            XElement currentElement = null;
            if (!e.MoveNext())
                currentElement = element.AddElement(new XElement(parts[0]));
            else
                currentElement = e.Current;

            if (parts.Length == 1) //the last link in the chain
                return currentElement;
            else
                return currentElement.Select(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        /// <summary>
        /// Gets WiX compatible string representation (e.g. HKCR, HKLM).
        /// </summary>
        /// <param name="value">The <see cref="T:Microsoft.Win32.RegistryHive"/> value to convert.</param>
        /// <returns>WiX compatible string representation.</returns>
        public static string ToWString(this RegistryHive value)
        {
            switch (value)
            {
                case RegistryHive.ClassesRoot: return "HKCR";
                case RegistryHive.CurrentUser: return "HKCU";
                case RegistryHive.LocalMachine: return "HKLM";
                case RegistryHive.Users: return "HKU";
                default: return "unsupported root type";
            }
        }

        /// <summary>
        /// Converts <see cref="T:WixSharp.Sequence"/> into the WiX identifier by removing WiX illegal characters.
        /// </summary>
        /// <param name="value">The <see cref="T:WixSharp.Sequence"/> value.</param>
        /// <returns>Valid WiX identifier.</returns>
        internal static string ToWString(this Sequence value)
        {
            return value.ToString().Replace(" ", "_");
        }

        /// <summary>
        /// Converts the string into the WiX identifier by removing WiX illegal characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Valid WiX identifier.</returns>
        internal static string ToWString(this string value)
        {
            return value.Replace(" ", "_");
        }

        /// <summary>
        /// Converts the string into the <see cref="T:WixSharp.Condition"/> instance.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns><see cref="T:WixSharp.Condition"/> instance.</returns>
        public static Condition ToCondition(this string value)
        {
            return Condition.Create(value);
        }

        /// <summary>
        /// Generates string representation without revision part.
        /// </summary>
        /// <param name="ver">The instance of the <see cref="T:System.Version"/>.</param>
        /// <returns><see cref="T:System.String"/></returns>
        public static string ToNoRevisionString(this Version ver)
        {
            return string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
        }
        /// <summary>
        /// Gets WiX compatible type name of the CLR <see cref="T:System.Object"/>. 
        /// This method should be used to generate string value of the <c>RegistryValue.Type</c> attribute.
        /// </summary>
        /// <param name="obj">The instance of the <see cref="T:System.Object"/>.</param>
        /// <returns>The WiX compatible type name.</returns>
        [Obsolete("It is no longer used by compiler")]
        public static string GetWType(this object obj)
        {
            if (obj is String)
            {
                return "string";
            }
            else if (obj is Int16 || obj is Int32)
            {
                return "integer";
            }
            else
            {
                return "unsupported type";
            }
        }
        /// <summary>
        /// Combines given <see cref="T:System.Array"/> items with items of another <see cref="T:System.Array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <c>obj</c>.</typeparam>
        /// <param name="obj">A <see cref="T:System.Array"/> whose elements to combine.</param>
        /// <param name="items">Another instance of <see cref="T:System.Array"/> whose elements to combine with <c>obj</c>.</param>
        /// <returns>A combined <see cref="T:System.Array"/>.</returns>
        public static T[] Combine<T>(this Array obj, Array items)
        {
            if (items != null && items.Length != 0)
            {
                var info = items.GetType();
                var retval = new ArrayList();

                foreach (var item in obj)
                    retval.Add(item);

                foreach (var item in items)
                    retval.Add(item);

                return (T[])retval.ToArray(typeof(T));
            }
            return (T[])obj;
        }

        /// <summary>
        /// Adds/combines given <see cref="T:System.Array"/> object with the specified item.
        /// </summary>
        /// <typeparam name="T1">The type of the elements of <c>obj</c>.</typeparam>
        /// <typeparam name="T2">The type of the elements of the items being added.</typeparam>
        /// <param name="obj">The instance of the <see cref="T:System.Array"/>.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>Combined <see cref="T:System.Array"/> object.</returns>
        public static T1[] Add<T1, T2>(this T1[] obj, T2 item) where T2 : T1
        {
            if (item != null)
            {
                var retval = new ArrayList();

                if (obj != null)
                    foreach (var i in obj)
                        retval.Add(i);

                retval.Add(item);

                return (T1[])retval.ToArray(typeof(T1));
            }
            return (T1[])obj;
        }

        /// <summary>
        /// Adds/combines given <see cref="T:System.Array"/> object with the specified items.
        /// </summary>
        /// <typeparam name="T1">The type of the elements of <c>obj</c>.</typeparam>
        /// <typeparam name="T2">The type of the elements of the items being added.</typeparam>
        /// <param name="obj">The instance of the <see cref="T:System.Array"/>.</param>
        /// <param name="items">The items to be added.</param>
        /// <returns>Combined <see cref="T:System.Array"/> object.</returns>
        public static T1[] AddRange<T1, T2>(this T1[] obj, IEnumerable<T2> items)
        {
            if (items != null)
            {
                var retval = new ArrayList();

                if (obj != null)
                    foreach (var i in obj)
                        retval.Add(i);

                if (items != null)
                    foreach (var i in items)
                        retval.Add(i);

                return (T1[])retval.ToArray(typeof(T1));
            }
            return (T1[])obj;
        }
        /// <summary>
        /// Combines given <see cref="T:System.Collections.Generic.List"/> items with items of another <see cref="T:System.Collections.Generic.List"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <c>obj</c>.</typeparam>
        /// <param name="obj">A <see cref="T:System.Collections.Generic.List"/>.</param>
        /// <param name="items">Another instance of <see cref="T:System.Collections.Generic.List"/> whose elements are to be combined with those of <c>obj</c>.</param>
        /// <returns>A combined <see cref="T:System.Collections.Generic.List"/>.</returns>
        public static List<T> Combine<T>(this List<T> obj, List<T> items)
        {
            if (items != null && items.Count != 0)
            {
                var retval = new List<T>();
                retval.AddRange(items);
                return retval;
            }
            return obj;
        }
        /// <summary>
        /// Fluent version of the <see cref="T:System.String.IsNullOrEmpty"/> for analysing the string value 
        /// for being <c>null</c> or empty.
        /// </summary>
        /// <param name="obj">A <see cref="T:System.String"/> whose value to analyse.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string obj)
        {
            return string.IsNullOrEmpty(obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> is active.
        /// <para>It is useful for checking if the session is terminated (e.g. in deferred custom actions).</para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static bool IsActive(this Session session)
        {
            //if (!session.IsClosed) //unfortunately isClosed is always false even for the deferred actions
            try
            {
                var test = session.Components; //it will throw for the deferred action
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the product associated with the session is installed.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static bool IsInstalled(this Session session)
        {
            return session.Property("Installed").IsNotEmpty();
        }

        /// <summary>
        /// Gets a value indicating whether the product is being installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if installing; otherwise, <c>false</c>.
        /// </value>
        static public bool IsInstalling(this Session session)
        {
            return !session.IsInstalled() && !session.IsUninstalling();
        }

        /// <summary>
        /// Gets a value indicating whether the product is being repaired.
        /// </summary>
        static public bool IsRepairing(this Session session)
        {
            return session.IsInstalled() && !session.IsUninstalling();
        }

        /// <summary>
        /// Gets a value indicating whether the product is being uninstalled.
        /// </summary>
        static public bool IsUninstalling(this Session session)
        {
            return session.Property("REMOVE").SameAs("All", true);
        }

        /// <summary>
        /// Determines whether this is basic UI level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        public static bool IsBasic(this InstallUIOptions level)
        {
            return (level & InstallUIOptions.Full) != InstallUIOptions.Full;
        }
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns the value of the named property of the specified <see cref="T:Microsoft.Deployment.WindowsInstaller.Session"/> object.
        /// <para>It can be uses as a generic way of accessing the properties as it redirects (transparently) access to the 
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.Session.CustomActionData"/> if the session is terminated (e.g. in deferred 
        /// custom actions).</para>
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string Property(this Session session, string name)
        {
            if (session.IsActive())
                return session[name];
            else
                return session.CustomActionData[name];
        }

        /// <summary>
        /// Saves the binary (from the Binary table) into the file.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <param name="file">The file.</param>
        public static void SaveBinary(this Session session, string binary, string file)
        {
            //If binary is accessed this way it will raise "stream handle is not valid" exception
            //object result = session.Database.ExecuteScalar("select Data from Binary where Name = 'Fake_CRT.msi'");
            //Stream s = (Stream)result;
            //using (FileStream fs = new FileStream(@"....\Wix# Samples\Simplified Bootstrapper\Fake CRT1.msi", FileMode.Create))
            //{
            //    int Length = 256;
            //    var buffer = new Byte[Length];
            //    int bytesRead = s.Read(buffer, 0, Length);
            //    while (bytesRead > 0)
            //    {
            //        fs.Write(buffer, 0, bytesRead);
            //        bytesRead = s.Read(buffer, 0, Length);
            //    }
            //}

            //however View approach is OK
            using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
            {
                sql.Execute();

                System.IO.Stream stream = sql.Fetch().GetStream(1);

                using (var fs = new System.IO.FileStream(file, System.IO.FileMode.Create))
                {
                    int Length = 256;
                    var buffer = new Byte[Length];
                    int bytesRead = stream.Read(buffer, 0, Length);
                    while (bytesRead > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        bytesRead = stream.Read(buffer, 0, Length);
                    }
                }
            }
        }

        /// <summary>
        /// Tries the read the binary (from the Binary table) into the byte array.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <returns></returns>
        public static byte[] TryReadBinary(this Session session, string binary)
        {
            try
            {
                return ReadBinary(session, binary);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read the binary (from the Binary table) into the byte array.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="binary">The binary.</param>
        /// <returns></returns>
        public static byte[] ReadBinary(this Session session, string binary)
        {
            //If binary is accessed this way it will raise "stream handle is not valid" exception
            //object result = session.Database.ExecuteScalar("select Data from Binary where Name = 'Fake_CRT.msi'");
            //Stream s = (Stream)result;
            //using (FileStream fs = new FileStream(@"....\Wix# Samples\Simplified Bootstrapper\Fake CRT1.msi", FileMode.Create))
            //{
            //    int Length = 256;
            //    var buffer = new Byte[Length];
            //    int bytesRead = s.Read(buffer, 0, Length);
            //    while (bytesRead > 0)
            //    {
            //        fs.Write(buffer, 0, bytesRead);
            //        bytesRead = s.Read(buffer, 0, Length);
            //    }
            //}

            //however View approach is OK
            using (var sql = session.Database.OpenView("select Data from Binary where Name = '" + binary + "'"))
            {
                sql.Execute();

                System.IO.Stream stream = sql.Fetch().GetStream(1);

                using (var ms = new System.IO.MemoryStream())
                {
                    int Length = 256;
                    var buffer = new Byte[Length];
                    int bytesRead = stream.Read(buffer, 0, Length);
                    while (bytesRead > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                        bytesRead = stream.Read(buffer, 0, Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Handles the errors in the specified action being executed. The all exceptions are caught and logged to the msi log.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="action">The action.</param>
        /// <returns><see cref="T:Microsoft.Deployment.WindowsInstaller.ActionResult.Success"/> if no errors detected, otherwise
        /// it returns <see cref="T:Microsoft.Deployment.WindowsInstaller.ActionResult.Failure"/>.
        /// </returns>
        public static ActionResult HandleErrors(this Session session, System.Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                session.Log(e.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        /// <summary>
        /// To a collection into WixObject that can be passed in the Project constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static WixObject ToWObject<T>(this IEnumerable<T> items) where T : WixObject
        {
            return new WixItems(items.Cast<WixObject>());
        }
    }

    /// <summary>
    /// 'Byte array to string' serialization methods.
    /// </summary>
    public static class SerializingExtensions
    {
        /// <summary>
        /// Decodes hexadecimal string representation into the byte array.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static byte[] DecodeFromHex(this string obj)
        {
            var data = new List<byte>();
            for (int i = 0; !string.IsNullOrEmpty(obj) && i < obj.Length; )
            {
                if (obj[i] == ',')
                {
                    i++;
                    continue;
                }
                data.Add(byte.Parse(obj.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                i += 2;
            }
            return data.ToArray();
        }

        /// <summary>
        /// Encodes byte array into its hexadecimal string representation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string EncodeToHex(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts bytes into text according the specified Encoding..
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static string GetString(this byte[] obj, Encoding encoding = null)
        {
            if (obj == null) return null;
            if (encoding == null)
                return Encoding.Default.GetString(obj);
            else
                return encoding.GetString(obj);
        }

        /// <summary>
        /// Gets the bytes of the text according the specified Encoding.
        /// </summary>
        /// <param name="obj">The text.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string obj, Encoding encoding = null)
        {
            if (encoding == null)
                return Encoding.Default.GetBytes(obj);
            else
                return encoding.GetBytes(obj);
        }
    }

    class WixItems : WixObject
    {
        public IEnumerable<WixObject> Items;
        public WixItems(IEnumerable<WixObject> items)
        {
            Items = items;
        }
    }

    internal static class XmlMapping
    {
        public static XAttribute[] MapToXmlAttributes(this WixEntity obj)
        {

            var result = new List<XAttribute>();


            var fields = obj.GetType()
                            .GetFields()
                            .Select(f => new
                            {
                                Field = f,
                                Value = f.GetValue(obj),
                                Attribute = (XmlAttribute)f.GetCustomAttributes(typeof(XmlAttribute), false)
                                            .FirstOrDefault(),
                            })
                            .Where(x => x.Attribute != null && x.Value != null)
                            .ToArray();

            foreach (var item in fields)
            {
                string xmlValue = item.Value.ToString();

                if (item.Value is bool?)
                {
                    var bulVal = (item.Value as bool?);
                    if (!bulVal.HasValue)
                        continue;
                    else
                        xmlValue = bulVal.Value.ToYesNo();
                }
                else if (item.Value is bool)
                {
                    xmlValue = ((bool)item.Value).ToYesNo();
                }

                result.Add(new XAttribute(
                            item.Attribute.Name ?? item.Field.Name,
                            xmlValue));
            }

            return result.ToArray();
        }
    }

    internal class XmlAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
