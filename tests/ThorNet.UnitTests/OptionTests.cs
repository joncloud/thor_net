using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class OptionTests
    {
        [InlineData("Test,--class=A,--classDefault=B,--method=Foo,--methodDefault=D", "A,B,Foo,D")]
        [InlineData("Test,-cA,-dB,-mFoo,-nD", "A,B,Foo,D")]
        [Theory]
        public async Task Option_Tests(string args, string values)
        {
            Target.OptionValues = new Options();
            await Thor.StartAsync<Target>(Utility.ToArray(args));

            var valuesList = Utility.ToArray(values);

            Assert.Equal(valuesList[0], Target.OptionValues.Class);
            Assert.Equal(valuesList[1], Target.OptionValues.ClassDefault);
            Assert.Equal(valuesList[2], Target.OptionValues.Method);
            Assert.Equal(valuesList[3], Target.OptionValues.MethodDefault);
        }

        [Fact]
        public async Task Help_ShouldIncludeHyphen_GivenAliasWithoutHyphen()
        {
            var lines = await GetHelpAsync();

            var actual = Assert.Single(
                lines.Where(line => line.Trim().StartsWith("-c"))
            ).Trim();

            var expected = "-c, [--class=CLASS]   # class desc";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Help_ShouldIncludeHyphen_GivenAliasWithHyphen()
        {
            var lines = await GetHelpAsync();

            var actual = Assert.Single(
                lines.Where(line => line.Trim().StartsWith("-n"))
            ).Trim();

            var expected = "-n, [--methodDefault=METHODDEFAULT] # method default help";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Help_ShouldIncludePossibleValues_GivenOptionEnumType()
        {
            var lines = (await GetHelpAsync()).ToArray();

            int i;
            for (i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("  -m"))
                {
                    break;
                }
            }

            Assert.NotEqual(i, lines.Length - 1);

            var actual = lines[i + 1];

            var expected = "                        # Possible values: Foo, Bar, FooBar";
            Assert.Equal(expected, actual);
        }

        static async Task<IEnumerable<string>> GetHelpAsync()
        {
            var terminal = new MockTerminal(100);

            var commandName = nameof(Thor.Help);
            var args = new[]
            {
                nameof(Target.Test)
            };
            await new Target(terminal).InvokeAsync(commandName, args);

            return terminal.GetLines();
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
            
            [Option("method", "m", "", EnumType = typeof(PossibleValues))]
            [Option("methodDefault", "-n", "method default help", DefaultValue = "MethodDefault")]
            public void Test()
            {
                OptionValues.Class = Option("class");
                OptionValues.ClassDefault = Option("classDefault");
                OptionValues.Method = Option("method");
                OptionValues.MethodDefault = Option("methodDefault");
            }
        }

        public enum PossibleValues
        {
            Foo,
            Bar,
            FooBar
        }
    }
}
