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
        public void Test(string argsList, int exitCode)
        {
            string[] args = Utility.ToArray(argsList);

            int actual = Thor.Start<Target>(args);
            Assert.Equal(exitCode, actual);
        }

        public class Target : Thor
        {
            public Target()
            {
                Subcommand<SubTarget>();
            }

            [Desc("", "")]
            public int Constant() => 100;
            [Desc("", "")]
            public Task<int> ConstantTask() => Task.FromResult(200);
            [Desc("", "")]
            public void Throws()
            {
                throw new InvalidOperationException();
            }
        }

        public class SubTarget : Thor
        {
            [Desc("", "")]
            public int Constant() => 101;
            [Desc("", "")]
            public Task<int> ConstantTask() => Task.FromResult(201);
            [Desc("", "")]
            public void Throws()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
