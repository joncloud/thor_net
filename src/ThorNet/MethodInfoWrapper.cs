using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThorNet {
	public class MethodInfoWrapper : ICommand { 
		private readonly MethodInfo _method;
		
		public MethodInfoWrapper(MethodInfo method) {
			_method = method;
			
			DescAttribute desc = _method.GetCustomAttribute<DescAttribute>();
			Description = desc?.Description;
			Example = desc?.Example;
		}
		
		public string Description { get; }
		public string Example { get; }
		public string Name { get { return _method.Name; } }
		public IEnumerable<MethodOptionAttribute> Options { get { return _method.GetCustomAttributes<MethodOptionAttribute>(); } }
		public IEnumerable<IParameter> Parameters { get { return _method.GetParameters().Select(p => new ParameterInfoWrapper(p)); } }
		public Type ReturnType { get { return _method.ReturnType; } }
		
		public object Invoke(object host, object[] parameters) {
			return _method.Invoke(host, parameters);
		}
	}
}
