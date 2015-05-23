
namespace WixSharp.Bootstrapper
{
    public class BootstrapperApplicationRef : WixEntity
    {
        
    }

    public class StandardBootstrapperApplication : BootstrapperApplicationRef
    {
        public static BootstrapperApplicationRef RtfLicense = new BootstrapperApplicationRef() { Id = "WixStandardBootstrapperApplication.RtfLicense" };
        public static BootstrapperApplicationRef HyperlinkLicense = new BootstrapperApplicationRef() { Id = "WixStandardBootstrapperApplication.HyperlinkLicense" };
    }
}