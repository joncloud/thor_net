using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ThorNet.UnitTests {
	public class ParameterInfoWrapperTests {
		
		[InlineData("AllDefaults", "a", typeof(int), true, 0)]
		[InlineData("AllDefaults", "b", typeof(string), true, "test")]
		[InlineData("AllDefaults", "c", typeof(char), true, 'd')]
		[InlineData("Basic", "a", typeof(int), false, null)]
		[InlineData("Basic", "b", typeof(string), false, null)]
		[InlineData("Basic", "c", typeof(char), false, null)]
		[InlineData("SomeDefaults", "a", typeof(int), false, null)]
		[InlineData("SomeDefaults", "b", typeof(string), false, null)]
		[InlineData("SomeDefaults", "c", typeof(char), true, 'd')]
		[Theory]
		public void Ctor_Tests(string methodName, string name, Type type, bool hasDefaultValue, object defaultValue) {
		      MethodInfo method = typeof(Helper).GetMethod(methodName);
			  
			  ParameterInfo[] parameterInfos = method.GetParameters();
			  
			  ParameterInfo parameterInfo = parameterInfos.Single(p => p.Name == name);
			  
			  ParameterInfoWrapper target = new ParameterInfoWrapper(parameterInfo);
			  
			  Assert.Equal(name, target.Name);
			  Assert.Equal(type, target.Type);
			  Assert.Equal(hasDefaultValue, target.HasDefaultValue);
			  Assert.Equal(defaultValue, target.DefaultValue);
		}
		
		public class Helper {
			public void AllDefaults(int a = 0, string b = "test", char c = 'd') { }
			public void Basic(int a, string b, char c) { }
			public void SomeDefaults(int a, string b, char c = 'd') { }
		}
	}
}