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
        
        private object[] BindArguments(List<string> textArgs) {
            ParameterInfo[] parameters = _method.GetParameters();
            
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
                ParameterInfo parameter = parameters[i];
                args[i] = TypeHelper.Convert(textArg, parameter.ParameterType);
            }
            
            // Account for optional arguments.
            List<string> missingBindings = new List<string>();
            if (textArgs.Count < args.Length) {
                for (i = 0; i < args.Length; i++) {
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
            
            object result = _method.Invoke(_host, arguments);
            
            if (typeof(Task).IsAssignableFrom(_method.ReturnType)) {
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