using System;
using System.Collections.Generic;
using System.Linq;

namespace ThorNet {
	public class ParameterBinder {
		public IEnumerable<string> Bind(List<string> textArgs, IParameter[] parameters, out object[] args) {
			args = new object[parameters.Length];
			for (int i = 0; i < textArgs.Count; i++) {
				string textArg = textArgs[i];
				IParameter parameter = parameters[i];
				args[i] = TypeHelper.Convert(textArg, parameter.Type);
			}
			
			// Account for optional arguments.
			List<string> missingBindings = new List<string>();
			if (textArgs.Count < args.Length) {
				for (int i = 0; i < args.Length; i++) {
					object arg = args[i];
					if (arg == null) {
						IParameter parameter = parameters[i];
						if (parameter.HasDefaultValue) {
							args[i] = parameter.DefaultValue;
						}
						else {
							missingBindings.Add(parameter.Name);
						}
					}
				}
			}
			
			return missingBindings;
		}
	}
}
