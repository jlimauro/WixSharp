using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    public class ManagedProject : Project
    {
        public ManagedProject()
        {
        }

        public ManagedProject(string name, params WixObject[] items)
            : base(name, items)
        {
        }

        public delegate void SetupEventHandler(SetupEventArgs e);

        public event SetupEventHandler Load;
        public event SetupEventHandler BeforeExecute;
        public event SetupEventHandler AfterExecute;
        public event SetupEventHandler Exit;

        bool preprocessed = false;

        void Bind<T>(Expression<Func<T>> expression, When when, Step step, bool elevated = false)
        {
            var name = Reflect.NameOf(expression);
            var handler = expression.Compile()();

            if (handler != null)
            {
                var handlerAsm = (handler as Delegate).Method.DeclaringType.Assembly.Location;

                if (!this.DefaultRefAssemblies.Contains(handlerAsm))
                    this.DefaultRefAssemblies.Add(handlerAsm);
                string asm = this.GetType().Assembly.Location;

                this.Properties = this.Properties.Add(new Property(name + "_ClientHandlers", GetHandlersInfo(handler as MulticastDelegate)));
                if (elevated)
                    this.Actions = this.Actions.Add(new ElevatedManagedAction("WixSharp_" + name + "_Action", asm, Return.check, When.Before, Step.AppSearch, Condition.Create("1"))
                    {
                        UsesProperties = name + "_ClientHandlers",
                    });
                else
                    this.Actions = this.Actions.Add(new ManagedAction("WixSharp_" + name + "_Action", asm, Return.check, When.Before, Step.AppSearch, Condition.Create("1")));
            }
        }

        override internal void Preprocess()
        {
            base.Preprocess();

            if (!preprocessed)
            {
                preprocessed = true;

                Bind(() => Load, When.Before, Step.AppSearch);
                Bind(() => BeforeExecute, When.Before, Step.InstallFiles);
                Bind(() => AfterExecute, When.Before, Step.InstallFiles);
                Bind(() => Exit, When.Before, Step.InstallFiles, true);
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
            catch (Exception e)
            {
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
            var eventArgs = new SetupEventArgs
            {
                Session = session
            };

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
    }
}