using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using IO=System.IO;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;

namespace WixSharp
{
    /// <summary>
    /// 
    /// </summary>
    public class ManagedProject : Project
    {
        //some materials to consider: http://cpiekarski.com/2012/05/18/wix-custom-action-sequencing/

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedProject"/> class.
        /// </summary>
        public ManagedProject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedProject"/> class.
        /// </summary>
        /// <param name="name">The name of the project. Typically it is the name of the product to be installed.</param>
        /// <param name="items">The project installable items (e.g. directories, files, registrty keys, Custom Actions).</param>
        public ManagedProject(string name, params WixObject[] items)
            : base(name, items)
        {
        }

        /// <summary>
        /// Event handler of the ManagedSetup for the MSI runtime events.
        /// </summary>
        /// <param name="e">The <see cref="SetupEventArgs"/> instance containing the event data.</param>
        public delegate void SetupEventHandler(SetupEventArgs e);

        /// <summary>
        /// Occurs before AppSearch standard action.
        /// </summary>
        public event SetupEventHandler Load;
        /// <summary>
        /// Occurs before InstallFiles standard action.
        /// </summary>
        public event SetupEventHandler BeforeInstall;
        /// <summary>
        /// Occurs after InstallFiles standard action. The event is fired from the elevated execution.
        /// </summary>
        public event SetupEventHandler AfterInstall;

        IManagedUI managedUI;

        public IManagedUI ManagedUI
        {
            get { return managedUI; }
            set
            {
                if (managedUI != value)
                {
                    if (managedUI != null)
                        managedUI.UnbindFrom(this);

                    managedUI = value;

                    if (managedUI != null)
                        managedUI.BindTo(this);
                }
            }
        }

        bool preprocessed = false;

        string thisAsm = typeof(ManagedProject).Assembly.Location;

        void Bind<T>(Expression<Func<T>> expression, When when, Step step, bool elevated = false)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            const string wixSharpProperties = "WIXSHARP_RUNTIME_DATA,WixSharp_BeforeInstall_Dialogs,WixSharp_AfterInstall_Dialogs,WixSharp_BeforeUninstall_Dialogs,WixSharp_AfterUninstall_Dialogs,WixSharp_BeforeRepair_Dialogs,WixSharp_AfterRepair_Dialogs";

            if (handler != null)
            {
                foreach (string handlerAsm in handler.GetInvocationList().Select(x => x.Method.DeclaringType.Assembly.Location))
                {
                    if (!this.DefaultRefAssemblies.Contains(handlerAsm))
                        this.DefaultRefAssemblies.Add(handlerAsm);
                }

                this.Properties = this.Properties.Add(new Property("WixSharp_{0}_Handlers".FormatInline(name), GetHandlersInfo(handler as MulticastDelegate)));

                string dllEntry = "WixSharp_{0}_Action".FormatInline(name);

                if (elevated)
                    this.Actions = this.Actions.Add(new ElevatedManagedAction(new Id(dllEntry), dllEntry, thisAsm, Return.check, when, step, Condition.Create("1"))
                    {
                        UsesProperties = "WixSharp_{0}_Handlers,{1},{2}".FormatInline(name, wixSharpProperties, DefaultDeferredProperties),
                    });
                else
                    this.Actions = this.Actions.Add(new ManagedAction(new Id(dllEntry), dllEntry, thisAsm, Return.check, when, step, Condition.Create("1")));
            }
        }

        /// <summary>
        /// The default properties mapped for use with the deferred custom actions. See <see cref="ManagedAction.UsesProperties"/> for the details.
        /// <para>The default value is "INSTALLDIR,UILevel"</para>
        /// </summary>
        public string DefaultDeferredProperties = "INSTALLDIR";

        void InjectDialogs(string name, ManagedDialogs dialogs)
        {
            if (dialogs.Any())
            {
                var dialogsInfo = new StringBuilder();

                foreach (var item in dialogs)
                {
                    if (!this.DefaultRefAssemblies.Contains(item.Assembly.Location))
                        this.DefaultRefAssemblies.Add(item.Assembly.Location);

                    var info = GetDialogInfo(item);

                    ValidateDialogInfo(info);
                    dialogsInfo.AppendLine(info);
                }
                this.AddProperty(new Property(name, dialogsInfo.ToString().Trim()));
            }
        }

        string PrepareResourceFile()
        {
            if (this.LocalizationFile.IsNotEmpty())
            {
                return this.LocalizationFile;
            }
            else
            {
                var tempDir = IO.Path.Combine(IO.Path.GetTempPath(), "WixSharp");
                if (!IO.Directory.Exists(tempDir))
                    IO.Directory.CreateDirectory(tempDir);

                var file = IO.Path.Combine(tempDir, "WixUI_en-us.wxl");

                IO.File.WriteAllBytes(file, Resources.Resources.WixUI_en_us);
                return file;
            }
        }

        override internal void Preprocess()
        {
            base.Preprocess();

            if (!preprocessed)
            {
                preprocessed = true;

                if (this.ManagedUI != null)
                    this.AddBinary(new Binary(new Id("WixSharp_UIText"), PrepareResourceFile()));

                string dllEntry = "WixSharp_InitRuntime_Action";

                this.AddAction(new ManagedAction(new Id(dllEntry), dllEntry, thisAsm, Return.check, When.Before, Step.AppSearch, Condition.Always));

                if (ManagedUI != null)
                {
                    InjectDialogs("WixSharp_BeforeInstall_Dialogs", ManagedUI.BeforeInstall);
                    InjectDialogs("WixSharp_AfterInstall_Dialogs", ManagedUI.AfterInstall);
                    InjectDialogs("WixSharp_BeforeUninstall_Dialogs", ManagedUI.BeforeUninstall);
                    InjectDialogs("WixSharp_AfterUninstall_Dialogs", ManagedUI.AfterUninstall);
                    InjectDialogs("WixSharp_BeforeRepair_Dialogs", ManagedUI.BeforeRepair);
                    InjectDialogs("WixSharp_AfterRepair_Dialogs", ManagedUI.AfterRepair);
                }

                Bind(() => Load, When.Before, Step.AppSearch);
                Bind(() => BeforeInstall, When.Before, Step.InstallFiles);
                Bind(() => AfterInstall, When.After, Step.InstallFiles, true);
            }
        }

        static void ValidateDialogInfo(string info)
        {
            try
            {
                GetDialog(info);
            }
            catch (Exception)
            {
                //may need to do extra logging; not important for now
                throw;
            }
        }

        static string GetDialogInfo(Type dialog)
        {
            var info = string.Format("{0}|{1}",
                                          dialog.Assembly.FullName,
                                          dialog.FullName);
            return info;
        }

        internal static Type GetDialog(string info)
        {
            string[] parts = info.Split('|');

            var assembly = System.Reflection.Assembly.Load(parts[0]);
            var type = assembly.GetTypes().Single(t => t.FullName == parts[1]);

            return type;
        }


        static void ValidateHandlerInfo(string info)
        {
            try
            {
                GetHandler(info);
            }
            catch (Exception)
            {
                //may need to do extra logging; not important for now
                throw;
            }
        }

        internal static string GetHandlersInfo(MulticastDelegate handlers)
        {
            var result = new StringBuilder();

            foreach (Delegate action in handlers.GetInvocationList())
            {
                var handlerInfo = string.Format("{0}|{1}|{2}",
                                     action.Method.DeclaringType.Assembly.FullName,
                                     action.Method.DeclaringType.FullName,
                                     action.Method.Name);

                ValidateHandlerInfo(handlerInfo);

                result.AppendLine(handlerInfo);
            }
            return result.ToString().Trim();
        }


        static MethodInfo GetHandler(string info)
        {
            string[] parts = info.Split('|');

            var assembly = System.Reflection.Assembly.Load(parts[0]);
            var type = assembly.GetTypes().Single(t => t.FullName == parts[1]);
            var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Static)
                             .Single(m => m.Name == parts[2]);

            return method;
        }

        static void InvokeClientHandler(string info, SetupEventArgs eventArgs)
        {
            MethodInfo method = GetHandler(info);

            if (method.IsStatic)
                method.Invoke(null, new object[] { eventArgs });
            else
                method.Invoke(Activator.CreateInstance(method.DeclaringType), new object[] { eventArgs });
        }

        internal static ActionResult InvokeClientHandlers(Session session, string eventName)
        {
            var eventArgs = Convert(session);

            try
            {
                string handlersInfo = session.Property("WixSharp_{0}_Handlers".FormatInline(eventName));

                foreach (string item in handlersInfo.Trim().Split('\n'))
                {
                    InvokeClientHandler(item.Trim(), eventArgs);
                    if (eventArgs.Result == ActionResult.Failure || eventArgs.Result == ActionResult.UserExit)
                        break;
                }

                eventArgs.SaveData();
            }
            catch { }
            return eventArgs.Result;
        }

        internal static ActionResult Init(Session session)
        {
            //Debugger.Launch();
            var data = new SetupEventArgs.AppData();
            try
            {
                try
                {
                    var bytes = session.TryReadBinary("WixSharp_UIText");
                    if (bytes != null)
                    {
                        var uiResources = new SetupEventArgs.ResourcesData();
                        uiResources.InitFromWxl(bytes);
                        data["UIText"] = uiResources.ToString();
                    }
                }
                catch { }


                data["Installed"] = session["Installed"];
                data["REMOVE"] = session["REMOVE"];
                data["UILevel"] = session["UILevel"];
                data["MsiWindow"] = GetMsiForegroundWindow(session["ProductName"]).ToString();
                data["IsMaintenance"] = session.GetMode(InstallRunMode.Maintenance).ToString();
                data["MsiFile"] = session.Database.FilePath;
            }
            catch (Exception e)
            {
                session.Log(e.Message);
            }

            session["WIXSHARP_RUNTIME_DATA"] = data.ToString();

            return ActionResult.Success;
        }

        static SetupEventArgs Convert(Session session)
        {
            //Debugger.Launch();
            var result = new SetupEventArgs { Session = session };
            try
            {
                string data = session.Property("WIXSHARP_RUNTIME_DATA");
                result.Data.InitFrom(data);
                string uiTextData = result.Data["UIText"];
                result.UIText.InitFrom(uiTextData);
            }
            catch (Exception e)
            {
                session.Log(e.Message);

            }
            return result;
        }

        const int SW_HIDE = 0;
        const int SW_SHOW = 1;
        const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowText(IntPtr wnd)
        {
            int length = GetWindowTextLength(wnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(wnd, sb, sb.Capacity);
            return sb.ToString();
        }

        static IntPtr GetMsiForegroundWindow(string productName)
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("msiexec").Where(p => p.MainWindowHandle != IntPtr.Zero))
                {
                    if (GetWindowText(proc.MainWindowHandle) == productName && IsWindowVisible(proc.MainWindowHandle))
                    {
                        ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                        SetForegroundWindow(proc.MainWindowHandle);
                        return proc.MainWindowHandle;
                    }
                }
            }
            catch { }

            //There is no warranty that MsiWindow will be found in "Change/Repair+UI" mode (from ARP) as 
            //the window in not owned by msiexec (like in "Install" mode).
            //Thus let's try primitive 'find'.
            return FindWindow(null, productName);
        }

    }

    internal static class SerializingExtensions
    {
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

        public static string EncodeToHex(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        public static string GetString(this byte[] obj, Encoding encoding = null)
        {
            if (obj == null) return null;
            if (encoding == null)
                return Encoding.Default.GetString(obj);
            else
                return encoding.GetString(obj);
        }

        public static byte[] GetBytes(this string obj, Encoding encoding = null)
        {
            if (encoding == null)
                return Encoding.Default.GetBytes(obj);
            else
                return encoding.GetBytes(obj);
        }
    }
}