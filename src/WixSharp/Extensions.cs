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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using IO = System.IO;
using System.Xml;

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
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid map entry</exception>
        public static Dictionary<string, string> ToDictionary(this string map)
        {
            var retval = new Dictionary<string, string>();
            if (!map.IsEmpty())
            {
                foreach (string pair in map.Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        string[] tokens = pair.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        string name = tokens[0].Trim();
                        string value = tokens[1].Trim();

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
        /// Sets the vaule of the attribute. This is a fluent version of XElement.SetAttributeValue. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement SetAttribute(this XElement obj, string name, string value)
        {
            obj.SetAttributeValue(name, value);
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
        /// Formats the specified string.
        /// </summary>
        /// <param name="obj">The string to format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this string obj, params object[] args)
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
        /// For the list of supported constants analyse <c>WixSharp.Compiler.EnvironmentConstantsMapping.Keys</c>.
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
        /// Converts the given string path into the full path.
        /// </summary>
        /// <param name="s">The string path.</param>
        /// <returns>The full string path.</returns>
        public static string ToFullPath(this string s)
        {
            return IO.Path.GetFullPath(s);
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
        public static XElement Select(this XElement element, string path)
        {
            string[] parts = path.Split('/');

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

        //not sure if required as it is not working correctly right now selects only top line branches
        ///// <summary>
        ///// Selects from the given element all child elements matching the specified path (e.g. <c>Select("Product/Package")</c>).
        ///// </summary>
        ///// <param name="element">The element to be searched.</param>
        ///// <param name="path">The path.</param>
        ///// <returns>The element matching the path.</returns>
        //private static XElement[] SelectAll(this XElement element, string path)
        //{
        //    string[] parts = path.Split('/');

        //    var query = (from el in element.Elements()
        //                 where el.Name.LocalName == parts[0]
        //                 select el);

        //    var e = query.GetEnumerator();

        //    if (!e.MoveNext())
        //        return new XElement[0];

        //    if (parts.Length == 1) //the last link in the chain
        //    {
        //        return query.ToArray();
        //    }
        //    else
        //        return e.Current.SelectAll(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        //}


        /// <summary>
        /// Flattened "view" of all elements with a given name.
        /// </summary>
        /// <param name="element">The element to be searched.</param>
        /// <param name="elementName">The element name.</param>
        /// <returns>The elements matching the name.</returns>
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
        /// <param name="obj">The instance of the <see cref="T:System.Array"/>.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>Combined <see cref="T:System.Array"/> object.</returns>
        public static T1[] Add<T1, T2>(this T1[] obj, T2 item) where T2: T1
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
        /// <typeparam name="T">The type of the elements of <c>obj</c>.</typeparam>
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

    class WixItems : WixObject
    {
        public IEnumerable<WixObject> Items;
        public WixItems(IEnumerable<WixObject> items)
        {
            Items = items;
        }
    }
}
