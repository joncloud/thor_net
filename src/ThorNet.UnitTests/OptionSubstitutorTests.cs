using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests {
	public class OptionSubstitutorTests {
		[InlineData("-1", "-1Single", "Single", "")]
		[InlineData("--one", "--one=Single", "Single", "")]
		[InlineData("-m,-m,-m", "-m1,-m2,-m3", "1,2,3", "")]
		[InlineData("-1,-2,-3", "-1one,-2two,-3three", "one,two,three", "")]
		[InlineData("--one,--two,--three", "--one=one,--two=two,--three=three", "one,two,three", "")]
		[InlineData("-1", "a,b,c,-1one", "one", "a b c")]
		[Theory]
		public void Substitute_Tests(string optionsList, string textArgsList, string valuesList, string remainingArgs) {
			OptionSubstitutor target = new OptionSubstitutor();
			
			List<string> textArgs = Utility.ToArray(textArgsList).ToList();
			
			Dictionary<string, MethodOption> options = new Dictionary<string, MethodOption>();
			string[] optionsArray = Utility.ToArray(optionsList);
			for (int i = 0; i < optionsArray.Length; i++) {
				string option = optionsArray[i];
				if (!options.ContainsKey(option)) {
					options.Add(option, new MethodOption(option));
				}
			}
			
			Helper host = new Helper();
			target.Substitute(host, options, textArgs);
			
			string[] values = valuesList.Split(',');
			int j = host.Options.Count;
			for (int i = 0; i < values.Length; i++) {
				string value = values[i];
				if (value != "") {
					MethodOption option = host.Options[--j];
					Assert.Equal(optionsArray[i], option.Alias);
					Assert.Equal(value, option.Value);
				}
			}
			
			string args = string.Join(" ", textArgs);
			Assert.Equal(remainingArgs, args);
		}
		
		public class Helper : IThor {
			public Helper() {
				Options = new List<MethodOption>();
			}
			
			public List<MethodOption> Options { get; }
			
			public void AddOption(string name, string value) {
				Options.Add(new MethodOption(name) { Value = value });
			}
			public bool HasOption(string name) { return false; }
		}
	}
}