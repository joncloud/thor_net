using System;
using Xunit;

namespace ThorNet.UnitTests {
	public class TypeHelperTests {
		
		[InlineData("false", typeof(bool), false)]
		[InlineData("true", typeof(bool), true)]
		[InlineData("123.456", typeof(double), 123.456)]
		[InlineData("1", typeof(Helper), Helper.One)]
		[InlineData("2", typeof(Helper), Helper.Two)]
		[InlineData("3", typeof(Helper), Helper.Three)]
		[InlineData("One", typeof(Helper), Helper.One)]
		[InlineData("Two", typeof(Helper), Helper.Two)]
		[InlineData("Three", typeof(Helper), Helper.Three)]
		[InlineData("0", typeof(int), 0)]
		[InlineData("123", typeof(int), 123)]
		[InlineData("456", typeof(int), 456)]
		[InlineData("", typeof(string), "")]
		[InlineData("abc", typeof(string), "abc")]
		[Theory]
		public void Convert_Tests(string text, Type target, object expected) {
			object actual = TypeHelper.Convert(text, target);
			Assert.Equal(expected, actual);
		}
		
		[InlineData(typeof(Helper), true)]
		[InlineData(typeof(LocalClass), false)]
		[InlineData(typeof(LocalEnum), true)]
		[InlineData(typeof(string), false)]
		[Theory]
		public void IsEnum_Tests(Type type, bool isEnum) {
			bool actual = TypeHelper.IsEnum(type);
			Assert.Equal(isEnum, actual);
		}
		
		public enum Helper {
			One = 1,
			Two = 2,
			Three = 3
		}
		
		public class LocalClass { }
		public enum LocalEnum { }
	}
}