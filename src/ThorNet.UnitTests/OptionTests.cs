using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class OptionTests
    {
        [InlineData("Test,--class=A,--classDefault=B,--method=C,--methodDefault=D", "A,B,C,D")]
        [InlineData("Test,-cA,-dB,-mC,-nD", "A,B,C,D")]
        [Theory]
        public void Option_Tests(string args, string values)
        {
            Target.OptionValues = new Options();
            Thor.Start<Target>(Utility.ToArray(args));

            var valuesList = Utility.ToArray(values);

            Assert.Equal(valuesList[0], Target.OptionValues.Class);
            Assert.Equal(valuesList[1], Target.OptionValues.ClassDefault);
            Assert.Equal(valuesList[2], Target.OptionValues.Method);
            Assert.Equal(valuesList[3], Target.OptionValues.MethodDefault);
        }

        public class Options
        {
            public string Class { get; set; }
            public string ClassDefault { get; set; }
            public string Method { get; set; }
            public string MethodDefault { get; set; }
        }

        [Option("class", "c", "")]
        [Option("classDefault", "d", "", DefaultValue = "ClassDefault")]
        public class Target : Thor
        {
            public static Options OptionValues { get; set; }

            [Desc("", "")]
            [Option("method", "m", "")]
            [Option("methodDefault", "n", "", DefaultValue = "MethodDefault")]
            public void Test()
            {
                OptionValues.Class = Option("class");
                OptionValues.ClassDefault = Option("classDefault");
                OptionValues.Method = Option("method");
                OptionValues.MethodDefault = Option("methodDefault");
            }
        }
    }
}
