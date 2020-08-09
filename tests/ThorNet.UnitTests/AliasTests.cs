using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests
{
    public class AliasTests
    {
        [Fact]
        public void Invoke_ShouldCallCommand_GivenAlias()
        {
            var message = Guid.NewGuid().ToString();
            
            var thor = new T();
            thor.Invoke("handle", new[] { message });

            Assert.Equal(message, thor.Message);
        }

        [Fact]
        public void Invoke_ShouldCallCommand_GivenMethodName()
        {
            var message = Guid.NewGuid().ToString();

            var thor = new T();
            thor.Invoke(nameof(T.Handle), new[] { message });

            Assert.Equal(message, thor.Message);
        }

        [Fact]
        public void Help_ShouldPrintAlias_GivenAlias()
        {
            var lines = GetHelp();

            var actual = Assert.Single(
                lines.Where(
                    line => line.StartsWith("  handle")
                )
            );
            var expected = "  handle MESSAGE # handles the given message";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Help_ShouldPrintMethodName_GivenNoAlias()
        {
            var lines = GetHelp();

            var actual = Assert.Single(
                lines.Where(
                    line => line.StartsWith("  NoAlias")
                )
            );
            var expected = "  NoAlias        # test for no alias";
            Assert.Equal(expected, actual);
        }

        static IEnumerable<string> GetHelp()
        {
            var terminal = new MockTerminal(100);

            var commandName = nameof(Thor.Help);
            var args = new string[0];
            new T(terminal).Invoke(commandName, args);

            return terminal.GetLines();
        }

        public class T : Thor
        {
            public T() : this(NullTerminal.Instance) { }
            public T(ITerminal terminal)
                : base(terminal)
            { }

            public string Message { get; private set; }

            [Alias("handle")]
            [Desc("handle MESSAGE", "handles the given message")]
            public void Handle(string message)
            {
                Message = message;
            }

            [Desc("NoAlias", "test for no alias")]
            public void NoAlias()
            {

            }
        }
    }
}
