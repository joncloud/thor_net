using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests
{
    public class ThorCommandTests
    {
        [InlineData(nameof(Target.RequiredParameter), (int)BindingResultType.Missing, "a", "b,c")]
        [InlineData(nameof(Target.RequiredParameter), (int)BindingResultType.Missing, "a,b", "c")]
        [InlineData(nameof(Target.Parsing), (int)BindingResultType.InvalidFormat, "123,b", "b")]
        [InlineData(nameof(Target.Parsing), (int)BindingResultType.InvalidFormat, "a,2016-05-21", "a")]
        [Theory]
        public void BindArguments_Throws(string commandName, int type, string argsList, string expectedMissing)
        {
            var args = new List<string>(argsList.Split(','));
            var target = new Target();

            var command = target.Commands[commandName];

            var ex = Assert.Throws<MismatchedBindingException>(() =>
            {
                command.BindArguments(args);
            });

            string actualMissing = string.Join(",", ex.BindingResults.Select(r => r.Name));
            Assert.Equal(expectedMissing, actualMissing);
            Assert.True(
                ex.BindingResults.All(r => r.Type == (BindingResultType)type), 
                "Unexpected binding result types.");
        }

        public class Target : Thor
        {
            [Desc("", "")]
            public void RequiredParameter(string a, string b, string c) { }

            [Desc("", "")]
            public void Parsing(int a, DateTime b) { }
        }
    }
}
