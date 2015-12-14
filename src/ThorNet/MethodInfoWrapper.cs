using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThorNet {
	/// <summary>
	/// Implements a command by wrapping <see cref="MethodInfo" />
	/// </summary>
	public class MethodInfoWrapper : ICommand { 
		private readonly MethodInfo _method;
		
		public MethodInfoWrapper(MethodInfo method) {
			_method = method;
			
			DescAttribute desc = _method.GetCustomAttribute<DescAttribute>();
			Description = desc?.Description;
			Example = desc?.Example;
		}
		
		/// <summary>
		/// Gets the description of the command.
		/// </summary>
		public string Description { get; }
        
		/// <summary>
		/// Gets an example of how to use the command.
		/// </summary>
		public string Example { get; }
        
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public string Name { get { return _method.Name; } }
        
		/// <summary>
		/// Gets the available method options.
		/// </summary>
		public IEnumerable<MethodOptionAttribute> Options { get { return _method.GetCustomAttributes<MethodOptionAttribute>(); } }
		
		/// <summary>
		/// Gets the parameters to the method in order.
		/// </summary>
		public IEnumerable<IParameter> Parameters { get { return _method.GetParameters().Select(p => new ParameterInfoWrapper(p)); } }
		
		/// <summary>
		/// Gets the return type of the command if applicable.
		/// </summary>
		public Type ReturnType { get { return _method.ReturnType; } }
		
		/// <summary>
		/// Invokes the command on the host with the specified parameters.
		/// </summary>
		/// <param name="host">The host target to invoke the command on.</param>
		/// <param name="parameters">The set of parameter values to invoke the command with.</param>
		/// <returns>The result of the command.</returns>
		public object Invoke(object host, object[] parameters) {
			return _method.Invoke(host, parameters);
		}
	}
}
