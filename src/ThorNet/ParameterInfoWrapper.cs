using System;
using System.Reflection;

namespace ThorNet {
	/// <summary>
	/// Implements a command by wrapping <see cref="ParameterInfo" />
	/// </summary>
	public class ParameterInfoWrapper : IParameter {
		private readonly ParameterInfo _parameter;
		
		public ParameterInfoWrapper(ParameterInfo parameter) {
			_parameter = parameter;
		}
		
		/// <summary>
		/// Gets the default value for the parameter if applicable.
		/// </summary>
		public object DefaultValue { get { return HasDefaultValue ? _parameter.DefaultValue : null; } }
		
		/// <summary>
		/// Gets whether there is a default value for the parameter.
		/// </summary>
		public bool HasDefaultValue { get { return _parameter.HasDefaultValue; } }
		
		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		public string Name { get { return _parameter.Name; } }
		
		/// <summary>
		/// Gets the data type for the parameter the value is expected to be.
		/// </summary>
		public Type Type { get { return _parameter.ParameterType; } }
	}

}