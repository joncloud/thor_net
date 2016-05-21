using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Xunit;

namespace ThorNet.UnitTests {
	public class ParameterBinderTests {
		
		[InlineData("1", "Types_Integer")]
		[InlineData("100", "Types_Integer")]
		[InlineData("1,2,3,4", "Types_FourIntegers")]
		[InlineData("Lorem", "Types_String")]
		[InlineData("Ipsum", "Types_String")]
		[InlineData("a,b,c,d", "Types_FourStrings")]
		[Theory]
		public void Bind_BindsByType(string textArgsList, string typeMethod) {
			ParameterBinder target = new ParameterBinder();
			
			List<string> textArgs = Utility.ToArray(textArgsList).ToList();
			
			IEnumerable<Type> types = GetData<IEnumerable<Type>>(typeMethod);
			IParameter[] parameters = types.Select(t => new TypeParameter(t)).ToArray();
			
			object[] args;
			var missingBindings = target.Bind(textArgs, parameters, out args);
			
			string[] actualArgs = args.Select(a => a.ToString()).ToArray();
			
			Assert.Equal(textArgs, actualArgs);
			Assert.Empty(missingBindings);
		}
		
		private static T GetData<T>(string methodName) {
			return (T)typeof(ParameterBinderTests)
				.GetMethod(methodName)
				.Invoke(null, new object[0]);
		}
		
		public static IEnumerable<Type> Types_FourIntegers() {
			yield return typeof(int);
			yield return typeof(int);
			yield return typeof(int);
			yield return typeof(int);
		}
		
		public static IEnumerable<Type> Types_FourStrings() {
			yield return typeof(string);
			yield return typeof(string);
			yield return typeof(string);
			yield return typeof(string);
		}
		
		public static IEnumerable<Type> Types_Integer() {
			yield return typeof(int);
		}
		
		public static IEnumerable<Type> Types_String() {
			yield return typeof(string);
		}
		
		public class TypeParameter : IParameter {
			public TypeParameter(Type type) {
				Type = type;
			}
			
			public object DefaultValue { get; }
			public bool HasDefaultValue { get; }
			public string Name { get; }
			public Type Type { get; }
		}
	}
}