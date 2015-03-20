using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace WixSharp.Test
{
    //none of the existing UnitTests are migrated yet from the old codebase
    public class GenericTest
    {
        [Fact]
        public void Test()
        {
            Compiler.GuidGenerator = GuidGenerators.Default;
            Compiler.GuidGenerator = GuidGenerators.Sequential;
            Compiler.GuidGenerator = (seed) => Guid.Parse("9e2974a1-9539-4c5c-bef7-80fc35b9d7b0");
            Compiler.GuidGenerator = (seed) => Guid.NewGuid();
        }

        [Fact]
        public void Should_Compare_CollectionByItems()
        {
            var itemsA = new[] { "a", "b" };
            var itemsC = new[] { "b", "c" };
            var itemsB = new List<string>(new [] { "a", "b" });
            
            Assert.Equal(itemsB.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(itemsC.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(new[] { "a" }.GetItemsHashCode(), itemsA.GetItemsHashCode());
        }


        [Fact]
        public void Should_Name_CustomActionsPSequentially()
        {
            //var project = new Project("CustomActionTest",
            //   new ManagedAction("MyAction", Return.check, When.Before, Step.LaunchConditions, Condition.NOT_Installed, Sequence.InstallUISequence),
            //   new ManagedAction("MyAction", Return.check, When.After, Step.InstallInitialize, Condition.NOT_Installed, Sequence.InstallUISequence));

            //var file = Compiler.BuildWxs(project);
        }


        void CodeFormattingTestPad()
        {
        }
    }

}
