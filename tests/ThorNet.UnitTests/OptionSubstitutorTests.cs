using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests
{
    public class OptionSubstitutorTests
    {
        [InlineData("-1", "-1Single", "Single", "", false)]
        [InlineData("--one", "--one=Single", "Single", "", false)]
        [InlineData("-m,-m,-m", "-m1,-m2,-m3", "1,2,3", "", false)]
        [InlineData("-1,-2,-3", "-1one,-2two,-3three", "one,two,three", "", false)]
        [InlineData("--one,--two,--three", "--one=one,--two=two,--three=three", "one,two,three", "", false)]
        [InlineData("-1", "a,b,c,-1one", "one", "a b c", false)]
        [InlineData("--one", "--one", "", "", true)]
        [Theory]
        public void Substitute_Tests(string optionsList, string textArgsList, string valuesList, string remainingArgs, bool allowFlag)
        {
            OptionSubstitutor target = new OptionSubstitutor();

            List<string> textArgs = Utility.ToArray(textArgsList).ToList();

            Dictionary<string, Option> options = new Dictionary<string, Option>();
            string[] optionsArray = Utility.ToArray(optionsList);
            for (int i = 0; i < optionsArray.Length; i++)
            {
                string option = optionsArray[i];
                if (!options.ContainsKey(option))
                {
                    options.Add(option, new Option(option) { AllowFlag = allowFlag });
                }
            }

            Helper host = new Helper();
            target.Substitute(host, options, textArgs);

            string[] values = valuesList.Split(',');
            int j = host.OptionValues.Count;
            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];
                if (value != "")
                {
                    Option option = host.OptionValues[--j];
                    Assert.Equal(optionsArray[i], option.Alias);
                    Assert.Equal(value, option.Value);
                }
            }

            string args = string.Join(" ", textArgs);
            Assert.Equal(remainingArgs, args);
        }

        internal class Helper : IThor
        {
            public Helper()
            {
                OptionValues = new List<Option>();
            }

            public IEnumerable<OptionAttribute> Options => new OptionAttribute[0];
            public List<Option> OptionValues { get; }

            public void AddOption(string name, string value)
            {
                OptionValues.Add(new Option(name) { Value = value });
            }
            public bool HasOption(string name) { return false; }
        }
    }
}