using System;

namespace ThorNet
{
    /// <summary>
    /// Provides a long description to a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LongDescAttribute : Attribute
    {
        public LongDescAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}
