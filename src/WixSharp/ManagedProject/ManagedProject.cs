using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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

        bool preprocessed = false;

        string thisAsm = typeof(ManagedProject).Assembly.Location;

        void Bind<T>(Expression<Func<T>> expression, When when, Step step, bool elevated = false)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            if (handler != null)
            {
                foreach (string handlerAsm in handler.GetInvocationList().Select(x => x.Method.DeclaringType.Assembly.Location))
                {
                    if (!this.DefaultRefAssemblies.Contains(handlerAsm))
                        this.DefaultRefAssemblies.Add(handlerAsm);
                }

                this.Properties = this.Properties.Add(new Property(name + "_ClientHandlers", GetHandlersInfo(handler as MulticastDelegate)));
                if (elevated)
                    this.Actions = this.Actions.Add(new ElevatedManagedAction("WixSharp_" + name + "_Action", thisAsm, Return.check, when, step, Condition.Create("1"))
                    {
                        UsesProperties = name + "_ClientHandlers,WIXSHARP_RUNTIME_DATA;" + DefaultDeferredProperties,
                    });
                else
                    this.Actions = this.Actions.Add(new ManagedAction("WixSharp_" + name + "_Action", thisAsm, Return.check, when, step, Condition.Create("1")));
            }
        }

        /// <summary>
        /// The default properties mapped for use with the deferred custom actions. See <see cref="ManagedAction.UsesProperties"/> for the details.
        /// <para>The default value is "INSTALLDIR,UILevel"</para>
        /// </summary>
        public string DefaultDeferredProperties = "INSTALLDIR";

        override internal void Preprocess()
        {
            base.Preprocess();

            if (!preprocessed)
            {
                preprocessed = true;

                this.AddAction(new ManagedAction("WixSharp_InitRuntime_Action", thisAsm, Return.check, When.Before, Step.AppSearch, Condition.Always));

                Bind(() => Load, When.Before, Step.AppSearch);
                Bind(() => BeforeInstall, When.Before, Step.InstallFiles);
                Bind(() => AfterInstall, When.After, Step.InstallFiles, true);
            }
        }

        internal static string GetHandlersInfo(MulticastDelegate handlers)
        {
            var result = new StringBuilder();

            foreach (Delegate action in handlers.GetInvocationList())
            {
                var handlerInfo = string.Format("{0}|{1}|{2}",
                                     action.Method.DeclaringType.Assembly.FullName,
                                     action.Method.DeclaringType.Name,
                                     action.Method.Name);

                ValidateHandlerInfo(handlerInfo);

                result.AppendLine(handlerInfo);
            }
            return result.ToString();
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

        static MethodInfo GetHandler(string info)
        {
            string[] parts = info.Split('|');

            var assembly = System.Reflection.Assembly.Load(parts[0]);
            var type = assembly.GetTypes().Single(t => t.Name == parts[1]);
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
                string handlersInfo = session.Property(eventName + "_ClientHandlers");


                foreach (string item in handlersInfo.Trim().Split('\n'))
                {
                    InvokeClientHandler(item.Trim(), eventArgs);
                    if (eventArgs.Result == ActionResult.Failure || eventArgs.Result == ActionResult.UserExit)
                        break;
                }
            }
            catch { }
            return eventArgs.Result;
        }

        internal static ActionResult Init(Session session)
        {
            var data = new SetupEventArgs.AppData();
            try
            {
                data["Installed"] = session["Installed"];
                data["REMOVE"] = session["REMOVE"];
                data["UILevel"] = session["UILevel"];
                data["MsiWindow"] = GetMsiForegroundWindow().ToString();
                data["IsMaintenance"] = session.GetMode(InstallRunMode.Maintenance).ToString();
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
            var result = new SetupEventArgs { Session = session };
            try
            {
                string data = session.Property("WIXSHARP_RUNTIME_DATA");
                result.Data.InitFrom(data);
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

        static IntPtr GetMsiForegroundWindow()
        {
            try
            {
                var proc = Process.GetProcessesByName("msiexec").Where(p => p.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
                if (proc != null)
                {
                    ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                    SetForegroundWindow(proc.MainWindowHandle);
                    return proc.MainWindowHandle;
                }
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}