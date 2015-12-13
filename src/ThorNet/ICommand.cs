using System;
using System.Collections.Generic;

namespace ThorNet {
	public interface ICommand {
		string Description { get; }
		string Example { get; }
		string Name { get; }
		IEnumerable<MethodOptionAttribute> Options { get; }
		IEnumerable<IParameter> Parameters { get; }
		Type ReturnType { get; }
		
		object Invoke(object host, object[] parameters);
	}
}