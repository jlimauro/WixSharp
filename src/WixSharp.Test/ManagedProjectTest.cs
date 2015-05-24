using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class ManagedProjectTest
    {
        [Fact]
        public void CanHandle_ResourceEncoding_Test()
        {
            //WixUI_en_us is a WiX source file that is just added to Wix# codebase as resource.
            //This file can easily come with the wrong encoding. So we need to unsure it can be parsed 
            //during the installation.
            var xml = Resources.Resources.WixUI_en_us.GetString(System.Text.Encoding.UTF8);
            XDocument.Parse(xml);
        }
    }
}