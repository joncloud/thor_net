using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class ExceptionHandlingTests
    {
        [Fact]
        public async Task Invoke_ShouldUnwrapTargetInvocationException_GivenThrownException()
        {
            var message = Guid.NewGuid().ToString();
            var terminal = new MockTerminal(100);
            await new T(terminal).InvokeAsync(nameof(T.Throw), new[] { message });

            var actual = Assert.Single(
                terminal.GetLines().Where(line => line.StartsWith("[ERROR]"))
            );

            var expected = $"[ERROR] {message}";
            Assert.Equal(expected, actual);
        }

        public class T : Thor
        {
            public T(ITerminal terminal)
                : base(terminal)
            {
            }

            public void Throw(string message)
            {
                throw new Exception(message);
            }
        }
    }
}
