using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    public class ManagedProjectTest
    {
        [Fact]
        public void CanHandle_ResourecEncoding_Test()
        {
            //WixUI_en_us is a WiX source file that is just added to Wix# codebase as resource.
            //This file can easily come with the wrong encoding. So we need to unsure it can be parsed 
            //during the installation.
            var xml = Resources.Resources.WixUI_en_us.GetString(System.Text.Encoding.UTF8);
            XDocument.Parse(xml);
        }
        
        [Fact]
        public void CanHandle_SessionUITextEncoding_Test()
        {
            //var uiResources = new SetupEventArgs.ResourcesData();
            //uiResources["jp"] = "学";

            //var uiData = new SetupEventArgs.AppData();
            //uiData["UIText"] = uiResources.ToString();

            //string serialized = uiData.ToString();
            //var reconstructedData = new SetupEventArgs.AppData().InitFrom(serialized);
            //var rconstructedResource = new SetupEventArgs.ResourcesData().InitFrom(reconstructedData["UIText"]);

            //Assert.Equal("学", rconstructedResource["jp"]);
        }
    }
}