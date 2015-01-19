using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    }

}
