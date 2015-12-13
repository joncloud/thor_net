using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThorNet {
    public class Thor : IThor {
        
        private Dictionary<string, ThorCommand> _commands;
        private Dictionary<string, List<string>> _options;
        
        public Thor() 
            : this(new ConsoleWrapper()) {
            _commands = LoadCommands();
            _options = new Dictionary<string, List<string>>();
        }
        
        public Thor(ITerminal terminal) {
            Terminal = terminal;
        }
        
        public ITerminal Terminal { get; }
        
        void IThor.AddOption(string name, string value) {
            List<string> values;
            if (!_options.TryGetValue(name, out values)) {
                values = new List<string>();
                _options.Add(name, values);
            }
            values.Add(value);
        }
        
        bool IThor.HasOption(string name) {
            return _options.ContainsKey(name);
        }
        
        [Desc("help [COMMAND]", "Describe available commands or one specific command")]
        public void help(string commandName = null) {
            string name = GetType().GetTypeInfo().Assembly.GetName().Name;
            
            if (commandName == null) {
                foreach (ThorCommand command in _commands.OrderBy(p => p.Key).Select(p => p.Value)) {
                    string message = $"  dnx {name} {command.Example}\t# {command.Description}";
                    if (message.Length > Terminal.Width) {
                        message = message.Substring(0, Terminal.Width - 9);
                        message += "...";
                    }
                    Terminal.WriteLine(message);
                }
            }
            else {
                ThorCommand command;
                if (_commands.TryGetValue(commandName, out command)) {
                    Terminal.WriteLine("Usage:");
                    Terminal.WriteLine($" dnx {name} {command.Example}");
                    Terminal.WriteLine();
                    
                    // Print the options.
                    MethodOptionAttribute[] options = command.Options.ToArray();
                    if (options.Any()) {
                        Terminal.WriteLine("Options:");
                        foreach (MethodOptionAttribute option in options) {
                            Terminal.WriteLine($"  {option.Alias}, [--{option.Name}={option.Name.ToUpper()}]\t# {option.Description}");
                        }
                        Terminal.WriteLine();
                    }
                    
                    Terminal.WriteLine(command.Description);
                }
                else {
                    Terminal.WriteLine($"Could not find command \"{commandName}\".");
                }
            }
        }
        
        internal void Invoke(string commandName, string[] args) {
            foreach (string invalid in _commands.Where(p => p.Value.Example == null).Select(p => p.Value.Name)) {
                Terminal.WriteLine($"[WARNING] Attempted to create command \"{invalid}\" without usage or description. Add Desc if you want this method to be available as command, or declare it as a non-public member.");   
            }
            
            ThorCommand command;
            if (_commands.TryGetValue(commandName, out command)) {
                command.Invoke(args);
            }
            else {
                Terminal.WriteLine($"Could not find command \"{commandName}\".");
            }
        }
        
        private Dictionary<string, ThorCommand> LoadCommands() {
            Dictionary<string, ThorCommand> commands = new Dictionary<string, ThorCommand>();
            
            Type type = GetType(); 
            
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => typeof(Thor).IsAssignableFrom(m.DeclaringType) &&
                                    !m.IsSpecialName)
                        .Select(m => new ThorCommand((IThor)this, new MethodInfoWrapper(m)))
                        .ToDictionary(c => c.Name);
        }
        
        protected string Option(string name, Func<string> defaultValue = null) {
            return Options(name).FirstOrDefault() ?? defaultValue?.Invoke();
        }
        
        protected T Option<T>(string name, Func<string, T> convert = null, Func<T> defaultValue = null) {
            return Options<T>(name, convert, defaultValue).FirstOrDefault();
        }
        
        protected IEnumerable<string> Options(string name) {
            List<string> options;
            if (_options.TryGetValue(name, out options)) {
                return options;
            }
            else {
                return new string[0]; 
            }
        }
        
        protected IEnumerable<T> Options<T>(string name, Func<string, T> convert = null, Func<T> defaultValue = null) {
            if (convert == null) {
                convert = s => TypeHelper.Convert<T>(s);
            }
            
            IEnumerable<string> options = Options(name);
            if (options.Any()) {
                return options.Select(convert);
            }
            else {
                if (defaultValue == null) {
                    return new T[0];
                }
                else {
                    return new [] { defaultValue() };
                }
            }
        }
        
        public static void Start<T>(string[] args)
            where T : Thor, new() {
                
            T thor = new T();
            
            string commandName;
            if (!args.Any()) {
                commandName = nameof(help);
            }
            else {
                commandName = args[0];
                args = args.Skip(1).ToArray();
            }
            
            thor.Invoke(commandName, args);
        }
    }
}