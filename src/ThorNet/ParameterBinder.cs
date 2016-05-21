using System;
using System.Collections.Generic;
using System.Linq;

namespace ThorNet {
	public class ParameterBinder {
		public IEnumerable<BindingResult> Bind(List<string> textArgs, IParameter[] parameters, out object[] args) {
			args = new object[parameters.Length];
            var results = new List<BindingResult>();
			for (int i = 0; i < textArgs.Count; i++) {
				string textArg = textArgs[i];
				IParameter parameter = parameters[i];
                try
                {
                    args[i] = TypeHelper.Convert(textArg, parameter.Type);
                }
                catch (FormatException)
                {
                    results.Add(new BindingResult(parameter.Name, BindingResultType.InvalidFormat));
                }
			}
			
			// Account for optional arguments.
			if (textArgs.Count < args.Length) {
				for (int i = 0; i < args.Length; i++) {
					object arg = args[i];
					if (arg == null) {
						IParameter parameter = parameters[i];
						if (parameter.HasDefaultValue) {
							args[i] = parameter.DefaultValue;
						}
						else {
							results.Add(new BindingResult(parameter.Name, BindingResultType.Missing));
						}
					}
				}
			}
			
			return results;
		}
	}
}
