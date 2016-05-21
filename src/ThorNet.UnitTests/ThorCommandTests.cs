using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ThorNet.UnitTests
{
    public class ThorCommandTests
    {
        [InlineData("a")]
        [InlineData("a,b")]
        [Theory]
        public void BindArguments_ThrowsMissingParameterException(string argsList)
        {
            var args = new List<string>(argsList.Split(','));
            var target = new Target();

            var command = target.Commands[nameof(Target.RequiredParameter)];

            Assert.Throws<MissingParameterException>(() =>
            {
                command.BindArguments(args);
            });
        }

        public class Target : Thor
        {
            public void RequiredParameter(string a, string b, string c) { }
        }
    }
}
