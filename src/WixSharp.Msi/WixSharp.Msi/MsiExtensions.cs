using System;
using System.Text;
using WindowsInstaller;

namespace WixSharp.UI
{
    static class MsiExtensions
    {
        public static void Invoke(Func<MsiError> action)
        {
            MsiError res = action();
            if (res != MsiError.NoError)
                throw new Exception(res.ToString());
        }

        public static IntPtr View(this IntPtr db, string sql)
        {
            IntPtr view = IntPtr.Zero;
            Invoke(() => MsiInterop.MsiDatabaseOpenView(db, sql, out view));
            Invoke(() => MsiInterop.MsiViewExecute(view, IntPtr.Zero));
            return view;
        }

        public static IntPtr NextRecord(this IntPtr view)
        {
            IntPtr record = IntPtr.Zero;
            Invoke(() => MsiInterop.MsiViewFetch(view, ref record));
            return record;
        }

        public static string GetString(this IntPtr record, uint fieldIndex)
        {
            uint valueSize = 2048;
            var builder = new StringBuilder((int)valueSize);
            Invoke(() => MsiInterop.MsiRecordGetString(record, fieldIndex, builder, ref valueSize));

            return builder.ToString();
        }

        public static int GetInt(this IntPtr record, uint fieldIndex)
        {
            return MsiInterop.MsiRecordGetInteger(record, fieldIndex);
        }

        public static void Close(this IntPtr view)
        {
            MsiInterop.MsiViewClose(view);
        }

        public static int ToInt(this string obj)
        {
            return int.Parse(obj);
        }

        //Needed to treated as "1 based" arrays as it is very hared to follow the MSDN documentation.
        //http://msdn.microsoft.com/en-us/library/windows/desktop/aa370573(v=vs.85).aspx
        //Yes it is inefficient but it saves dev time and effort for translating C# data structures into MSI string fields
        public static T MSI<T>(this string[] obj, int fieldIndex)
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)(obj[fieldIndex - 1] == "0");
            else if (typeof(T) == typeof(int))
                return (T)(object)(obj[fieldIndex - 1].ToInt());
            else if (typeof(T) == typeof(string))
                return (T)(object)(obj[fieldIndex - 1]);

            //else if (typeof(T) == typeof(char))
            //{
            //    if (obj[fieldIndex + 1].Length == 1)
            //        return (T)(object)(obj[fieldIndex + 1])[0];
            //    else
            //        throw new Exception("Value contains more than a single character");
            //}
            else
                throw new Exception("Only Int32, String and Boolean conversion is supported");
        }
    }
}