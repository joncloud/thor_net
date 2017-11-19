using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ThorNet.UnitTests
{
    public class MethodInfoWrapperTests
    {

        [InlineData(nameof(Helper.HasLongDescription), "", "", "LongDescription", typeof(void))]
        [InlineData(nameof(Helper.Int), "", "", "", typeof(int))]
        [InlineData(nameof(Helper.String), "", "", "", typeof(string))]
        [InlineData(nameof(Helper.Void), "", "", "", typeof(void))]
        [InlineData(nameof(Helper.Void_WithDescription), "Lorem", "", "", typeof(void))]
        [InlineData(nameof(Helper.Void_WithDescriptionAndExample), "Ipsum", "Dolor", "", typeof(void))]
        [InlineData(nameof(Helper.Void_WithExample), "", "Sit", "", typeof(void))]
        [Theory]
        public void Ctor_Tests(string name, string description, string example, string longDescription, Type returnType)
        {
            MethodInfoWrapper target = Create(name);

            Assert.Equal(name, target.Name);
            Assert.Equal(description, target.Description);
            Assert.Equal(example, target.Example);
            Assert.Equal(longDescription, target.LongDescription);
            Assert.Equal(returnType, target.ReturnType);
        }

        [InlineData("Int", 0)]
        [InlineData("Int", 1)]
        [InlineData("Int", 2)]
        [InlineData("String", null)]
        [InlineData("String", "a")]
        [InlineData("String", "abc")]
        [InlineData("Void", null)]
        [Theory]
        public void Invoke_Tests(string name, object expected)
        {
            MethodInfoWrapper target = Create(name);

            Helper helper = new Helper(expected);

            object actual = target.Invoke(helper, new object[0]);

            Assert.Equal(expected, actual);
        }

        [InlineData("Void_WithMethodOption", "alpha,omega", "a,o", "beginning,ending")]
        [Theory]
        public void Options_Tests(string name, string namesList, string aliasesList, string descriptionsList)
        {
            MethodInfoWrapper target = Create(name);

            string[] actualNames = target.Options.Select(o => o.Name).ToArray();
            string[] actualAliases = target.Options.Select(o => o.Alias).ToArray();
            string[] actualDescriptions = target.Options.Select(o => o.Description).ToArray();

            string[] expectedNames = Utility.ToArray(namesList);
            string[] expectedAliases = Utility.ToArray(aliasesList);
            string[] expectedDescriptions = Utility.ToArray(descriptionsList);

            Assert.Equal(expectedNames, actualNames);
            Assert.Equal(expectedAliases, actualAliases);
            Assert.Equal(expectedDescriptions, actualDescriptions);
        }

        [InlineData("Void", "")]
        [InlineData("Void_WithParametersX", "x")]
        [InlineData("Void_WithParametersXY", "x,y")]
        [InlineData("Void_WithParametersXYZ", "x,y,z")]
        [Theory]
        public void Parameters_Tests(string name, string parametersList)
        {
            MethodInfoWrapper target = Create(name);

            Assert.True(target.Parameters.All(p => p is ParameterInfoWrapper), "MethodInfoWrapper does not use ParameterInfoWrapper.");

            string[] actual = target.Parameters.Select(p => p.Name).ToArray();
            string[] expected = Utility.ToArray(parametersList);

            Assert.Equal(expected, actual);
        }

        private static MethodInfoWrapper Create(string name)
        {
            MethodInfo method = typeof(Helper).GetMethod(name);

            return new MethodInfoWrapper(method);
        }

        public class Helper
        {
            private object _result;
            public Helper(object result)
            {
                _result = result;
            }

            [LongDesc("LongDescription")]
            public void HasLongDescription() { }
            public int Int() { return (int)_result; }
            public string String() { return (string)_result; }
            public void Void() { }
            [Desc(null, "Lorem")]
            public void Void_WithDescription() { }
            [Desc("Dolor", "Ipsum")]
            public void Void_WithDescriptionAndExample() { }
            [Desc("Sit", null)]
            public void Void_WithExample() { }

            [Option("alpha", "a", "beginning")]
            [Option("omega", "o", "ending")]
            public void Void_WithMethodOption() { }
            public void Void_WithParametersX(int x) { }
            public void Void_WithParametersXY(int x, int y) { }
            public void Void_WithParametersXYZ(int x, int y, int z) { }
        }
    }
}
