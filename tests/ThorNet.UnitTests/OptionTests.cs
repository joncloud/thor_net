using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void Help_ShouldIncludeHyphen_GivenAliasWithoutHyphen()
        {
            var lines = GetHelp();

            var actual = Assert.Single(
                lines.Where(line => line.Trim().StartsWith("-c"))
            ).Trim();

            var expected = "-c, [--class=CLASS]   # class desc";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Help_ShouldIncludeHyphen_GivenAliasWithHyphen()
        {
            var lines = GetHelp();

            var actual = Assert.Single(
                lines.Where(line => line.Trim().StartsWith("-n"))
            ).Trim();

            var expected = "-n, [--methodDefault=METHODDEFAULT] # method default help";
            Assert.Equal(expected, actual);
        }

        static IEnumerable<string> GetHelp()
        {
            var terminal = new MockTerminal(100);

            var commandName = nameof(Thor.help);
            var args = new[]
            {
                nameof(Target.Test)
            };
            new Target(terminal).Invoke(commandName, args);

            var lines = terminal.ToString()
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }

        public class Options
        {
            public string Class { get; set; }
            public string ClassDefault { get; set; }
            public string Method { get; set; }
            public string MethodDefault { get; set; }
        }

        [Option("class", "c", "class desc")]
        [Option("classDefault", "d", "", DefaultValue = "ClassDefault")]
        public class Target : Thor
        {
            public Target()
                : base(NullTerminal.Instance)
            {
            }

            public Target(ITerminal terminal)
                : base(terminal)
            {
            }

            public static Options OptionValues { get; set; }
            
            [Option("method", "m", "")]
            [Option("methodDefault", "-n", "method default help", DefaultValue = "MethodDefault")]
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
