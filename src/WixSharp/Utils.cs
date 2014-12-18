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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using IO = System.IO;

namespace WixSharp
{
    internal static class Utils
    {
        //fix for unexpected behaviour: System.IO.Path.Combine(@"C:\Test", @"\Docs\readme.txt") return @"\Docs\readme.txt";
        internal static string PathCombine(string path1, string path2)
        {
            if (path2.Length == 0)
            {
                return path1;
            }
            else if (path2.Length == 1 && path2[0] == IO.Path.DirectorySeparatorChar)
            {
                return path1;
            }
            else if (path2[0] == IO.Path.DirectorySeparatorChar)
            {
                if (path2[0] != path2[1])
                    return IO.Path.Combine(path1, path2.Substring(1));
            }

            return IO.Path.Combine(path1, path2);
        }

        internal static string[] AllConstStringValues<T>()
        {
            var fields = typeof(T).GetFields()
                                  .Where(f => f.IsStatic && f.IsPublic && f.IsLiteral && f.FieldType == typeof(string))
                                  .Select(f=>f.GetValue(null) as string)
                                  .ToArray();

            return fields;
        }

        /// <summary>
        /// Gets the program files directory.
        /// </summary>
        /// <value>
        /// The program files directory.
        /// </value>
        internal static string ProgramFilesDirectory 
        {
            get 
            {
                string programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                if ("".GetType().Assembly.Location.Contains("Framework64"))
                    programFilesDir += " (x86)"; //for x64 systems
                return programFilesDir;
            }
        }

        //needed to have reliable HASH as x64 and x32 have different algorithms; This leads to the inability of script clients calculate cache directory correctly  
        public static int GetHashCode32(this string s)
        {
            char[] chars = s.ToCharArray();
            int lastCharInd = chars.Length - 1;
            int num1 = 0x15051505;
            int num2 = num1;
            int ind = 0;
            while (ind <= lastCharInd)
            {
                char ch = chars[ind];
                char nextCh = ++ind > lastCharInd ? '\0' : chars[ind];
                num1 = (((num1 << 5) + num1) + (num1 >> 0x1b)) ^ (nextCh << 16 | ch);
                if (++ind > lastCharInd)
                    break;
                ch = chars[ind];
                nextCh = ++ind > lastCharInd ? '\0' : chars[ind++];
                num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ (nextCh << 16 | ch);
            }
            return num1 + num2 * 0x5d588b65;
        }
    }
}
