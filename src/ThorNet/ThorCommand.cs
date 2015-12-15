using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ThorNet {
	public class ThorCommand {
		
		private readonly ICommand _command;
		private readonly IThor _host;
		private readonly MethodOptionAttribute[] _options;
		
		public ThorCommand(IThor host, ICommand command) {
			_command = command;
			_host = host;
			_options = _command.Options.ToArray();
		}
		
		public string Description { get { return _command.Description; } }
		public string Example { get { return _command.Example; } }
		public string Name { get { return _command.Name; } }
		public IEnumerable<MethodOptionAttribute> Options { get { return _options; } }
		
		private object[] BindArguments(List<string> textArgs) {
			IParameter[] parameters = _command.Parameters.ToArray();
			
			// Map the options.
			Dictionary<string, MethodOption> options = GetOptions();
			OptionSubstitutor substitutor = new OptionSubstitutor();
			substitutor.Substitute(_host, options, textArgs); 
			
			// Convert the arguments.
			ParameterBinder binder = new ParameterBinder();
			object[] args;
			string[] missingBindings = binder.Bind(textArgs, parameters, out args).ToArray();
			
			if (missingBindings.Any()) {
				throw new AmbiguousMatchException($"Mismatched parameter(s): {string.Join(", ", missingBindings)}.");
			}
			
			return args;
		}
		
		private Dictionary<string, MethodOption> GetOptions() {
			return _options.SelectMany(o => {
				var option = new MethodOption(o.Name);
				
				return new [] { 
					// Map using the alias (-1) and full name (--one=).
					new { Key = o.Alias, Option = option },
					new { Key = "--" + o.Name, Option = option }
				};
			})
			 .ToDictionary(x => x.Key, x => x.Option);
		}
		
		public void Invoke(string[] args) {
			object[] arguments = BindArguments(args.ToList());
			
			// Provide default values for options..
			foreach (MethodOptionAttribute option in _options) {
				if (option.DefaultValue != null &&
					!_host.HasOption(option.Name)) {
					_host.AddOption(option.Name, option.DefaultValue);
				}
			}
			
			object result = _command.Invoke(_host, arguments);
		   
			// If the result is a task, then make sure to wait for it to complete.  
			if (typeof(Task).IsAssignableFrom(_command.ReturnType)) {
				Task task = (Task)result;
				task.Wait();
			}
		}
	}
}