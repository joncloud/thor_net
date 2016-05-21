using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThorNet
{
    public class ThorCommand {
		
		private readonly ICommand _command;
		private readonly IThor _host;
		private readonly OptionAttribute[] _options;
        private readonly ITerminal _terminal;
		
		public ThorCommand(IThor host, ICommand command, ITerminal terminal) {
			_command = command;
			_host = host;
			_options = _command.Options.Concat(host.Options).ToArray();
            _terminal = terminal;
		}
		
		public string Description { get { return _command.Description; } }
		public string Example { get { return _command.Example; } }
		public string Name { get { return _command.Name; } }
		public IEnumerable<OptionAttribute> Options { get { return _options; } }
		
		internal object[] BindArguments(List<string> textArgs) {
			IParameter[] parameters = _command.Parameters.ToArray();
			
			// Map the options.
			Dictionary<string, Option> options = GetOptions();
			OptionSubstitutor substitutor = new OptionSubstitutor();
			substitutor.Substitute(_host, options, textArgs); 
			
			// Convert the arguments.
			ParameterBinder binder = new ParameterBinder();
			object[] args;
			var results = binder.Bind(textArgs, parameters, out args).ToArray();
			
			if (results.Any()) {
				throw new MismatchedBindingException(
                    results,
                    $"Mismatched parameter(s): {string.Join(", ", results.Select(r => r.Name))}.");
			}
			
			return args;
		}
		
		private Dictionary<string, Option> GetOptions() {
			return _options.SelectMany(o => {
				var option = new Option(o);
				
				return new [] { 
					// Map using the alias (-1) and full name (--one=).
					new { Key = "-" + o.Alias, Option = option },
					new { Key = "--" + o.Name, Option = option }
				};
			})
			 .ToDictionary(x => x.Key, x => x.Option);
		}
		
		public void Invoke(string[] args) {
            try
            {
                object[] arguments = BindArguments(args.ToList());

                // Provide default values for options..
                foreach (OptionAttribute option in _options.Concat(_host.Options))
                {
                    if (option.DefaultValue != null &&
                        !_host.HasOption(option.Name))
                    {
                        _host.AddOption(option.Name, option.DefaultValue);
                    }
                }

                object result = _command.Invoke(_host, arguments);

                // If the result is a task, then make sure to wait for it to complete.  
                if (typeof(Task).GetTypeInfo().IsAssignableFrom(_command.ReturnType))
                {
                    Task task = (Task)result;
                    task.Wait();
                }
            }
            catch (MismatchedBindingException ex)
            {
                _terminal.WriteLine($"\"{Name}\" was called incorrectly. Call as \"{Example}\"");

                const string prefix = "  ";
                int max = ex.BindingResults.Max(r => r.Name.Length) + prefix.Length;
                StringBuilder message = new StringBuilder();
                foreach (var result in ex.BindingResults)
                {
                    message.Append(prefix)
                        .Append(result.Name.ToUpper());

                    if (message.Length < max)
                    {
                        int delta = max - message.Length;
                        message.Append(' ', delta);
                    }
                    message.Append(" # ");

                    switch (result.Type)
                    {
                        case BindingResultType.InvalidFormat:
                            message.Append("Invalid format");
                            break;
                        case BindingResultType.Missing:
                            message.Append("Missing parameter");
                            break;
                        default:
                            message.Append("Unknown compatibility");
                            break;
                    }

                    _terminal.Truncate(message);
                    _terminal.WriteLine(message.ToString());
                    message.Clear();
                }
            }
        }
	}
}