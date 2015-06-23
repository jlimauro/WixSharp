using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using Xunit;

namespace WixSharp.Test
{
    public class GenericTest
    {
        [Fact]
        public void RelativePath()
        {
            var path = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Content\readme.txt";
            var baseDir = @"E:\Projects\WixSharp\src\WixSharp.Samples\Wix# Samples\Install Files";

            var result = Utils.MakeRelative(path, baseDir);
            Assert.Equal(@"..\Content\readme.txt", result);
        }
        
        [Fact]
        public void EnumExtensions()
        {
            var test = ConfigureServiceTrigger.Install | ConfigureServiceTrigger.Reinstall;

            Assert.True(ConfigureServiceTrigger.Install.PresentIn(test));
            Assert.False(ConfigureServiceTrigger.Uninstall.PresentIn(test));
        }

        [Fact]
        public void ArrayExtensions()
        {
            var project = new Project();
            project.Actions = new Action[] { new ManagedAction() };
            //should not throw
            project.Actions = project.Actions.Add(new InstalledFileAction("", ""));
            project.Actions = project.Actions.AddRange(project.Actions);
        }

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
            var itemsB = new List<string>(new[] { "a", "b" });

            Assert.Equal(itemsB.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(itemsC.GetItemsHashCode(), itemsA.GetItemsHashCode());
            Assert.NotEqual(new[] { "a" }.GetItemsHashCode(), itemsA.GetItemsHashCode());
        }

        [Fact]
        public void Should_Combine_Sequences()
        {
            var s1 = Sequence.InstallUISequence;
            var s2 = Sequence.InstallExecuteSequence;

            var result1 = s1 + s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result1.ToString());

            var result2 = s1 | s2;
            Assert.Equal("InstallUISequence|InstallExecuteSequence", result2.ToString());
        }

[Fact]
void StringEnum_Test()
{
    Assert.Equal("newVal", new MyVals("newVal"));
    Assert.Equal("firstVal", MyVals.First);
    Assert.Equal("secondVal", MyVals.Second);
    Assert.Equal("thirdVal", MyVals.Third);


    Assert.True(MyVals.Third == "thirdVal");
    Assert.True("thirdVal" == MyVals.Third);

    Assert.False(MyVals.Third == "");
    Assert.True(MyVals.Third != "");
            
    Assert.False("" == MyVals.Third);
    Assert.True("" != MyVals.Third);

    Assert.Equal("thirdVal", MyVals.Third);
    Assert.Equal("thirdVal", (string)MyVals.Third);

    Assert.True(MyVals.Third.Equals("thirdVal"));
    Assert.False(MyVals.Third.Equals(null));
    Assert.True(MyVals.Third.Equals(new MyVals("thirdVal")));
    Assert.True(MyVals.Third.Equals(MyVals.Third));

}

class MyVals : StringEnum<MyVals>
{
    public MyVals(string value) : base(value) { }

    public static MyVals First = new MyVals("firstVal");
    public static MyVals Second = new MyVals("secondVal");
    public static MyVals Third = new MyVals("thirdVal");
}
    }
}
