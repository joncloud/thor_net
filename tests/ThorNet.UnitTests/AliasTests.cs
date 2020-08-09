using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class AliasTests
    {
        [Fact]
        public async Task Invoke_ShouldCallCommand_GivenAlias()
        {
            var message = Guid.NewGuid().ToString();
            
            var thor = new T();
            await thor.InvokeAsync("handle", new[] { message });

            Assert.Equal(message, thor.Message);
        }

        [Fact]
        public async Task Invoke_ShouldCallCommand_GivenMethodName()
        {
            var message = Guid.NewGuid().ToString();

            var thor = new T();
            await thor.InvokeAsync(nameof(T.Handle), new[] { message });

            Assert.Equal(message, thor.Message);
        }

        [Fact]
        public async Task Help_ShouldPrintAlias_GivenAlias()
        {
            var lines = await GetHelpAsync();

            var actual = Assert.Single(
                lines.Where(
                    line => line.StartsWith("  handle")
                )
            );
            var expected = "  handle MESSAGE # handles the given message";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Help_ShouldPrintMethodName_GivenNoAlias()
        {
            var lines = await GetHelpAsync();

            var actual = Assert.Single(
                lines.Where(
                    line => line.StartsWith("  NoAlias")
                )
            );
            var expected = "  NoAlias        # test for no alias";
            Assert.Equal(expected, actual);
        }

        static async Task<IEnumerable<string>> GetHelpAsync()
        {
            var terminal = new MockTerminal(100);

            var commandName = nameof(Thor.Help);
            var args = new string[0];
            await new T(terminal).InvokeAsync(commandName, args);

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
