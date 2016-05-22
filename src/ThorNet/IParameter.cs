using System;

namespace ThorNet
{
    /// <summary>
    /// Provides an interface for defining a parameter to a command.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets the default value for the parameter if applicable.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Gets whether there is a default value for the parameter.
        /// </summary>
        bool HasDefaultValue { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the data type for the parameter the value is expected to be.
        /// </summary>
        Type Type { get; }
    }
}