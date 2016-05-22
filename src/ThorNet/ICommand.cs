using System;
using System.Collections.Generic;

namespace ThorNet
{
    /// <summary>
    /// Provides an interface for defining command-based work.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets an example of how to use the command.
        /// </summary>
        string Example { get; }

        /// <summary>
        /// Gets the long description of the command.
        /// </summary>
        string LongDescription { get; }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the available method options.
        /// </summary>
        IEnumerable<OptionAttribute> Options { get; }

        /// <summary>
        /// Gets the parameters to the method in order.
        /// </summary>
        IEnumerable<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the return type of the command if applicable.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Invokes the command on the host with the specified parameters.
        /// </summary>
        /// <param name="host">The host target to invoke the command on.</param>
        /// <param name="parameters">The set of parameter values to invoke the command with.</param>
        /// <returns>The result of the command.</returns>
        object Invoke(object host, object[] parameters);
    }
}