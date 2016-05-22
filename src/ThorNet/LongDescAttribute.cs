using System;

namespace ThorNet
{
    /// <summary>
    /// Provides a long description to a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LongDescAttribute : Attribute
    {
        /// <summary>
        /// Creates a new long description attribute.
        /// </summary>
        public LongDescAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Gets the long description for the command.
        /// </summary>
        public string Description { get; }
    }
}
