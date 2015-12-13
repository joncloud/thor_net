using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ThorNet.UnitTests {
	public class MethodInfoWrapperTests {
		
		[InlineData("Int", null, null, typeof(int))]
		[InlineData("String", null, null, typeof(string))]
		[InlineData("Void", null, null, typeof(void))]
		[InlineData("Void_WithDescription", "Lorem", null, typeof(void))]
		[InlineData("Void_WithDescriptionAndExample", "Ipsum", "Dolor", typeof(void))]
		[InlineData("Void_WithExample", null, "Sit", typeof(void))]
		[Theory]
		public void Ctor_Tests(string name, string description, string example, Type returnType) {
			MethodInfo method = typeof(Helper).GetMethod(name);
			
			MethodInfoWrapper target = new MethodInfoWrapper(method);
			
			Assert.Equal(name, target.Name);
			Assert.Equal(description, target.Description);
			Assert.Equal(example, target.Example);
			Assert.Equal(returnType, target.ReturnType);
		}
		
		[InlineData("Void_WithMethodOption", "alpha,omega", "a,o", "beginning,ending")]
		public void Options_Tests(string name, string namesList, string aliasesList, string descriptionsList) {
			MethodInfo method = typeof(Helper).GetMethod(name);
			
			MethodInfoWrapper target = new MethodInfoWrapper(method);
			
			string[] actualNames = target.Options.Select(o => o.Name).ToArray();
			string[] actualAliases = target.Options.Select(o => o.Alias).ToArray();
			string[] actualDescriptions = target.Options.Select(o => o.Description).ToArray();
			
			string[] expectedNames = FromList(namesList);
			string[] expectedAliases = FromList(aliasesList);
			string[] expectedDescriptions = FromList(descriptionsList);
			
			Assert.Equal(expectedNames, actualNames);
			Assert.Equal(expectedAliases, actualAliases);
			Assert.Equal(expectedDescriptions, actualDescriptions);
		}
		
		[InlineData("Void", "")]
		[InlineData("Void_WithParametersX", "x")]
		[InlineData("Void_WithParametersXY", "x,y")]
		[InlineData("Void_WithParametersXYZ", "x,y,z")]
		public void Parameters_Tests(string name, string parametersList) {
			MethodInfo method = typeof(Helper).GetMethod(name);
			
			MethodInfoWrapper target = new MethodInfoWrapper(method);
		
			Assert.True(target.Parameters.All(p => p is ParameterInfoWrapper), "MethodInfoWrapper does not use ParameterInfoWrapper.");
			
			string[] actual = target.Parameters.Select(p => p.Name).ToArray();
			string[] expected = FromList(parametersList);
			
			Assert.Equal(expected, actual);
		}
		
		private static string[] FromList(string list) {
			return list.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}
		
		public class Helper {
			public int Int() { return 0; }
			public string String() { return ""; }
			public void Void() { }
			[Desc(null, "Lorem")]
			public void Void_WithDescription() { }
			[Desc("Dolor", "Ipsum")]
			public void Void_WithDescriptionAndExample() { }
			[Desc("Sit", null)]
			public void Void_WithExample() { }
			
			[MethodOption("alpha", "a", "beginning")]
			[MethodOption("omega", "o", "ending")]
			public void Void_WithMethodOption() { }
			public void Void_WithParametersX(int x) { }
			public void Void_WithParametersXY(int x, int y) { }
			public void Void_WithParametersXYZ(int x, int y, int z) { }
		}
	}
}
