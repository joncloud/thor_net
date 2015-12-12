using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ThorNet {
	public class ThorCommand {
        
        private readonly Thor _host;
        private readonly MethodInfo _method;
        private readonly MethodOptionAttribute[] _options;
        
        public ThorCommand(Thor host, MethodInfo method) {
            _host = host;
            _method = method;
            
            DescAttribute desc = _method.GetCustomAttribute<DescAttribute>();
            if (desc != null) {
                Description = desc.Description;
                Example = desc.Example;
            }
            
            _options = _method.GetCustomAttributes<MethodOptionAttribute>().ToArray();
        }
        
        public string Description { get; }
        public string Example { get; }
        public string Name { get { return _method.Name; } }
        public IEnumerable<MethodOptionAttribute> Options { get { return _options; } }
        
        private object[] BindArguments(string[] textArgs) {
            ParameterInfo[] parameters = _method.GetParameters();
            Dictionary<string, int> aliases = GetAliases(parameters);
            
            object[] args = new object[parameters.Length];
            
            for (int i = 0; i < textArgs.Length; i++) {
                int target = i;
                string textArg = textArgs[i];
                SubstituteAlias(aliases, ref textArg, ref target);
                
                ParameterInfo parameter = parameters[target];
                args[target] = Convert.ChangeType(textArg, parameter.ParameterType);
            }
            
            List<string> missingBindings = new List<string>();
            if (textArgs.Length < args.Length) {
                for (int i = 0; i < args.Length; i++) {
                    object arg = args[i];
                    if (arg == null) {
                        ParameterInfo parameter = parameters[i];
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
        
        private Dictionary<string, int> GetAliases(ParameterInfo[] parameters) {
            Dictionary<string, int> aliases = parameters.Select((p, i) => new { p, i })
                                                        .ToDictionary(x => "--" + x.p.Name, x => x.i);
            
            foreach (MethodOptionAttribute option in _options) {
                aliases.Add(option.Alias, aliases["--" + option.Name]);
            }
            
            return aliases;
        }
        
        public void Invoke(string[] args) {
            object result = _method.Invoke(_host, BindArguments(args));
            
            if (typeof(Task).IsAssignableFrom(_method.ReturnType)) {
                Task task = (Task)result;
                task.Wait();
            }
        }
        
        private void SubstituteAlias(Dictionary<string, int> aliases, ref string text, ref int target) {
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
                    if (aliases.TryGetValue(alias, out target)) {
                        text = textValue; 
                    }
                    else { 
                        // TODO
                    }
                }
            }
        }
    }
}