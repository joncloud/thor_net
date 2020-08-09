using System;

namespace ThorNet
{
    /// <summary>
    /// Provides an alternate name for calling a method from the CLI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Creates a new alias attribute.
        /// </summary>
        public AliasAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// Gets the alias to use instead of the method name.
        /// </summary>
        public string Alias { get; }
    }
}