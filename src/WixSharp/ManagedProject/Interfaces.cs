using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 1591

namespace WixSharp
{
    public interface IManagedDialog
    {
    }
    public interface IManagedProgressDialog:IManagedDialog
    {
    }

    public interface IManagedUI
    {
        ManagedDialogs InstallDialogs { get; }
        ManagedDialogs RepairDialogs { get; }
    }

    public class ManagedDialogs : List<Type>
    {
        //public ManagedDialogs Add(Type dialog)
        //{
        //    if (dialog.FindInterfaces()
        //    base.Add(dialog);
        //    return this;
        //}

        public ManagedDialogs Add<T>() where T : IManagedDialog
        {
            base.Add(typeof(T));
            return this;
        }

        public new ManagedDialogs Clear()
        {
            base.Clear();
            return this;
        }
    }
}
