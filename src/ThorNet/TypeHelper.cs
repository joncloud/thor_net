using System;
using System.Reflection;

namespace ThorNet
{
    internal static class TypeHelper
    {
        public static T Convert<T>(string text) => (T)Convert(text, typeof(T));

        public static object Convert(string text, Type target)
        {
            if (IsEnum(target))
            {
                return Enum.Parse(target, text);
            }
            return System.Convert.ChangeType(text, target);
        }

        public static bool IsEnum(Type type) => type.GetTypeInfo().IsEnum;
    }
}
