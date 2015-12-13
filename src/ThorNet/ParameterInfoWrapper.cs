using System;
using System.Reflection;

namespace ThorNet {

	public class ParameterInfoWrapper : IParameter {
		private readonly ParameterInfo _parameter;
		
		public ParameterInfoWrapper(ParameterInfo parameter) {
			_parameter = parameter;
		}
		
		public object DefaultValue { get { return _parameter.DefaultValue; } }
		public bool HasDefaultValue { get { return _parameter.HasDefaultValue; } }
		public string Name { get { return _parameter.Name; } }
		public Type Type { get { return _parameter.ParameterType; } }
	}

}