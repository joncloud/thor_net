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
            
            object[] args = new object[parameters.Length];
            
            // Map the options.
            Dictionary<string, MethodOption> options = GetOptions();
            int i = textArgs.Count;
            while (--i >= 0) {
                string textArg = textArgs[i];
                MethodOption option;
                if (TrySubstituteOption(options, textArg, out option)) {
                    textArgs.RemoveAt(i);
                    _host.AddOption(option.Alias, option.Value);
                }
            }
            
            // Convert the arguments.
            for (i = 0; i < textArgs.Count; i++) {
                string textArg = textArgs[i];
                IParameter parameter = parameters[i];
                args[i] = TypeHelper.Convert(textArg, parameter.Type);
            }
            
            // Account for optional arguments.
            List<string> missingBindings = new List<string>();
            if (textArgs.Count < args.Length) {
                for (i = 0; i < args.Length; i++) {
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
            
            if (missingBindings.Any()) {
                throw new AmbiguousMatchException($"Mismatched parameter(s): {string.Join(", ", missingBindings)}.");
            }
            
            return args;
        }
        
        private Dictionary<string, MethodOption> GetOptions() {
            return _options.SelectMany(o => {
                var option = new MethodOption(o.Name);
                
                return new [] { 
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
            
            if (typeof(Task).IsAssignableFrom(_command.ReturnType)) {
                Task task = (Task)result;
                task.Wait();
            }
        }
        
        private bool TrySubstituteOption(Dictionary<string, MethodOption> options, string text, out MethodOption option) {
            option = null;
            if (text.Length > 0 && text[0] == '-') {
                string alias;
                string textValue;
                
                int position;
                int offset = 0;
                if (text.Length > 2 && text[1] == '-') {
                    position = text.IndexOf("=");
                    offset = 1;
                }
                else {
                    position = 2;
                }
                
                if (position > 0) {
                    alias = text.Substring(0, position);
                    textValue = text.Substring(position + offset, text.Length - position - offset);
                }
                else { 
                    alias = null;
                    textValue = null; 
                }
                
                if (alias != null) {
                    if (options.TryGetValue(alias, out option)) {
                        option.Value = textValue;
                        return true; 
                    }
                    else { 
                        return false;
                    }
                }
            }
            
            return false;
        }
        
        private class MethodOption {
            public MethodOption(string alias) {
                Alias = alias;
            }
            public string Alias { get; }
            public string Value { get; set; }
        }
    }
}