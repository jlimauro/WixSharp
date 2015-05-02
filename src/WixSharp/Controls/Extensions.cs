using System;
using System.Security.Principal;
using Wix = WixSharp;

namespace WixSharp
{
    public static partial class Extensions
    {
        internal static Wix.Control ConvertToWControl(this IWixControl srcControl, ControlType controlType)
        {
            var wControl = new Wix.Control { Type = controlType.ToString() };

            wControl.CopyCommonPropertiesFrom(srcControl);

            return wControl;
        }

        static public bool IsAdmin(this WindowsIdentity identity)
        {
            var p = new WindowsPrincipal(identity);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// The WiX UI scaling factor.
        /// WiX pixels are not the same as Windows ones. For example Wix.Point(56,17) is equivalent of the Win.Point(75,23)
        /// </summary>
        static public double WixScalingFactor = 0.756;

        /// <summary>
        /// Converts (scales) Windows coordinate value into WiX one.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static int WScale(this int data)
        {
            return (int)(data * WixScalingFactor);
        }


        internal static void CopyCommonPropertiesFrom(this Wix.Control destControl, IWixControl srcControl)
        {
            var formControl = (System.Windows.Forms.Control)srcControl;

            if (srcControl.Conditions != null)
                destControl.Conditions.AddRange(srcControl.Conditions);

            destControl.Height = formControl.Size.Height.WScale();
            destControl.Width = formControl.Size.Width.WScale();
            destControl.X = formControl.Left.WScale();
            destControl.Y = formControl.Top.WScale();

            destControl.Disabled = !formControl.Enabled;
            destControl.Hidden = srcControl.Hidden;
            destControl.AttributesDefinition = srcControl.WixAttributes;
            destControl.Property = srcControl.BoundProperty;

            if (srcControl is WixControl)
                destControl.EmbeddedXML = (srcControl as WixControl).EmbeddedXML;

            if (!formControl.Text.IsEmpty())
                destControl.Text = formControl.Text;

            if (!srcControl.Tooltip.IsEmpty())
                destControl.Tooltip = srcControl.Tooltip;

            destControl.Name = formControl.Name.IsNullOrEmpty() ? destControl.Type : formControl.Name;
            if (!srcControl.Id.IsNullOrEmpty())
                destControl.Id = srcControl.Id; //destControl.Id is a calculated property and if not set explicitly it will fell back to the destControl.Name

            if (srcControl is IWixInteractiveControl)
            {
                destControl.Actions.AddRange((srcControl as IWixInteractiveControl).Actions);
            }
        }
    }
}