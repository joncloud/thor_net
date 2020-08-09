using System;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests
{
    public class ExceptionHandlingTests
    {
        [Fact]
        public void Invoke_ShouldUnwrapTargetInvocationException_GivenThrownException()
        {
            var message = Guid.NewGuid().ToString();
            var terminal = new MockTerminal(100);
            new E(terminal).Invoke(nameof(E.Throw), new[] { message });

            var actual = Assert.Single(
                terminal.GetLines().Where(line => line.StartsWith("[ERROR]"))
            );

            var expected = $"[ERROR] {message}";
            Assert.Equal(expected, actual);
        }

        public class E : Thor
        {
            public E(ITerminal terminal)
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
