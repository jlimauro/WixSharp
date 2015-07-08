using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;
using IO = System.IO;
using System.IO;

#pragma warning disable 1591

namespace WixSharp
{
    /// <summary>
    ///
    /// </summary>
    public partial class ManagedProject : Project
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

        public IManagedUI ManagedUI;

        bool preprocessed = false;

        string thisAsm = typeof(ManagedProject).Assembly.Location;

        void Bind<T>(Expression<Func<T>> expression, When when, Step step, bool elevated = false)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()() as Delegate;

            const string wixSharpProperties = "WIXSHARP_RUNTIME_DATA";

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

        override internal void Preprocess()
        {
            base.Preprocess();

            if (!preprocessed)
            {
                preprocessed = true;

                //It is too late to set prerequisites. Launch conditions are evaluated after UI is popped up.   
                //this.SetNetFxPrerequisite(Condition.Net35_Installed, "Please install .NET v3.5 first.");

                string dllEntry = "WixSharp_InitRuntime_Action";

                this.AddAction(new ManagedAction(new Id(dllEntry), dllEntry, thisAsm, Return.check, When.Before, Step.AppSearch, Condition.Always));

                if (ManagedUI != null)
                {
                    ManagedUI.EmbeddResourcesInto(this);

                    InjectDialogs("WixSharp_InstallDialogs", ManagedUI.InstallDialogs);
                    InjectDialogs("WixSharp_ModifyDialogs", ManagedUI.ModifyDialogs);

                    this.EmbeddedUI = new EmbeddedAssembly(ManagedUI.GetType().Assembly.Location);
                }

                Bind(() => Load, When.Before, Step.AppSearch);
                Bind(() => BeforeInstall, When.Before, Step.InstallFiles);
                Bind(() => AfterInstall, When.After, Step.InstallFiles, true);
            }
        }

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
                    dialogsInfo.Append(info + "\n");
                }
                this.AddProperty(new Property(name, dialogsInfo.ToString().Trim()));
            }
        }

        public static List<Type> ReadDialogs(string data)
        {
            return data.Split('\n')
                       .Select(x => x.Trim())
                       .Where(x => x.IsNotEmpty())
                       .Select(x => ManagedProject.GetDialog(x))
                       .ToList();
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
            var info = "{0}|{1}".FormatInline(
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
                var handlerInfo = "{0}|{1}|{2}".FormatInline(
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
                data["Installed"] = session["Installed"];
                data["REMOVE"] = session["REMOVE"];
                data["UILevel"] = session["UILevel"];
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
            }
            catch (Exception e)
            {
                session.Log(e.Message);
            }
            return result;
        }
    }
}
