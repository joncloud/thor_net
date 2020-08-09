using System;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    /// <summary>
    /// Tests to make sure subcommands are run.
    /// </summary>
    public class SubcommandTests
    {
        [Fact]
        public void ConflictingCommandAndSubcommand_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new DuplicateTarget();
            });
        }

        public class DuplicateTarget : Thor
        {
            public DuplicateTarget()
            {
                Subcommand<Duplicate>();
            }

            public void Duplicate() { }
        }

        public class Duplicate : Thor
        {
        }

        [Fact]
        public async Task Subcommand_IsTriggered()
        {
            Assert.Equal(0, Trigger.Counter);
            await Thor.StartAsync<TriggerTarget>(new[] { nameof(Trigger), nameof(Trigger.Increment) });
            Assert.Equal(1, Trigger.Counter);
        }

        public class TriggerTarget : Thor
        {
            public TriggerTarget()
                : base(NullTerminal.Instance)
            {
                Subcommand<Trigger>();
            }
        }

        public class Trigger : Thor
        {
            public Trigger()
                : base(NullTerminal.Instance)
            {
            }

            public static int Counter => _counter;
            static int _counter;

            public void Increment() { _counter++; }
        }
    }
}
