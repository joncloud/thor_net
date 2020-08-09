using System;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class ExitTests
    {
        [InlineData("", 0)]
        [InlineData("help", 0)]
        [InlineData("help,Constant", 0)]
        [InlineData("help,SubTarget", 0)]
        [InlineData("help,SubTarget,Constant", 0)]
        [InlineData("help,SubTarget,Invalid", 1)]
        [InlineData("help,Invalid", 1)]
        [InlineData("Constant", 100)]
        [InlineData("ConstantTask", 200)]
        [InlineData("Invalid", 1)]
        [InlineData("Throws", 1)]
        [InlineData("SubTarget,Constant", 101)]
        [InlineData("SubTarget,ConstantTask", 201)]
        [InlineData("SubTarget,Invalid", 1)]
        [InlineData("SubTarget,Throws", 1)]
        [Theory]
        public async Task Test(string argsList, int exitCode)
        {
            string[] args = Utility.ToArray(argsList);

            int actual = await Thor.StartAsync<Target>(args);
            Assert.Equal(exitCode, actual);
        }

        public class Target : Thor
        {
            public Target()
                : base(NullTerminal.Instance)
            {
                Subcommand<SubTarget>();
            }
            
            public int Constant() => 100;
            public Task<int> ConstantTask() => Task.FromResult(200);
            public void Throws()
            {
                throw new InvalidOperationException();
            }
        }

        public class SubTarget : Thor
        {
            public SubTarget()
                : base(NullTerminal.Instance)
            {
            }
            
            public int Constant() => 101;
            public Task<int> ConstantTask() => Task.FromResult(201);
            public void Throws()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
