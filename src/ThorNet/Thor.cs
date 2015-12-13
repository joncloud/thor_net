using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThorNet {
    public class Thor {
        
        private Dictionary<string, ThorCommand> _commands;
        private Dictionary<string, List<string>> _options;
        
        public Thor() {
            _commands = LoadCommands();
            _options = new Dictionary<string, List<string>>();
        }
        
        internal void AddOption(string name, string value) {
            List<string> values;
            if (!_options.TryGetValue(name, out values)) {
                values = new List<string>();
                _options.Add(name, values);
            }
            values.Add(value);
        }
        
        internal bool HasOption(string name) {
            return _options.ContainsKey(name);
        }
        
        [Desc("help [COMMAND]", "Describe available commands or one specific command")]
        public void help(string commandName = null) {
            string name = GetType().GetTypeInfo().Assembly.GetName().Name;
            
            if (commandName == null) {
                foreach (ThorCommand command in _commands.OrderBy(p => p.Key).Select(p => p.Value)) {
                    string message = $"  dnx {name} {command.Example}\t# {command.Description}";
                    if (message.Length > Console.WindowWidth) {
                        message = message.Substring(0, Console.WindowWidth - 9);
                        message += "...";
                    }
                    Console.WriteLine(message);
                }
            }
            else {
                ThorCommand command;
                if (_commands.TryGetValue(commandName, out command)) {
                    Console.WriteLine("Usage:");
                    Console.WriteLine($" dnx {name} {command.Example}");
                    Console.WriteLine();
                    
                    // Print the options.
                    MethodOptionAttribute[] options = command.Options.ToArray();
                    if (options.Any()) {
                        Console.WriteLine("Options:");
                        foreach (MethodOptionAttribute option in options) {
                            Console.WriteLine($"  {option.Alias}, [--{option.Name}={option.Name.ToUpper()}]\t# {option.Description}");
                        }
                        Console.WriteLine();
                    }
                    
                    Console.WriteLine(command.Description);
                }
                else {
                    Console.WriteLine($"Could not find command \"{commandName}\".");
                }
            }
        }
        
        internal void Invoke(string commandName, string[] args) {
            foreach (string invalid in _commands.Where(p => p.Value.Example == null).Select(p => p.Value.Name)) {
                Console.WriteLine($"[WARNING] Attempted to create command \"{invalid}\" without usage or description. Add desc if you want this method to be available as command, or declare it inside no_commands block.");   
            }
            
            ThorCommand command;
            if (_commands.TryGetValue(commandName, out command)) {
                command.Invoke(args);
            }
            else {
                Console.WriteLine($"Could not find command \"{commandName}\".");
            }
        }
        
        private Dictionary<string, ThorCommand> LoadCommands() {
            Dictionary<string, ThorCommand> commands = new Dictionary<string, ThorCommand>();
            
            Type type = GetType(); 
            
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => typeof(Thor).IsAssignableFrom(m.DeclaringType))
                        .Select(m => new ThorCommand(this, m))
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