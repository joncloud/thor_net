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
        
        /// <summary>
        /// Adds an option's value by name.
        /// </summary>
        /// <param name="name">The name of the option to provide a value for.</param>
        /// <param name="value">The value to provide.</param>
        void IThor.AddOption(string name, string value) {
            List<string> values;
            if (!_options.TryGetValue(name, out values)) {
                values = new List<string>();
                _options.Add(name, values);
            }
            values.Add(value);
        }

        /// <summary>
        /// Gets the first option and determines if the value was specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>True of the option was specified, otherwise false.</returns>
        protected bool Flag(string name)
        {
            return Option(name) != null;
        }

        /// <summary>
        /// Gets the name of the package for display in the <see cref="help(string)"/> method.
        /// </summary>
        protected virtual string GetPackageName()
        {
            string name = GetType().GetTypeInfo().Assembly.GetName().Name;

            return $"dnx {name}";
        }

        /// <summary>
        /// Determines if the option has already been specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>True if there is a value for the option, otherwise false.</returns>
        bool IThor.HasOption(string name) {
            return _options.ContainsKey(name);
        }
        
        [Desc("help [COMMAND]", "Describe available commands or one specific command")]
        public void help(string commandName = null) {
            string name = GetPackageName();
            
            // Print all of the commands.
            if (commandName == null) {
                foreach (ThorCommand command in _commands.OrderBy(p => p.Key).Select(p => p.Value)) {
                    string message = $"  {name} {command.Example}\t# {command.Description}";
                    if (message.Length > Terminal.Width) {
                        message = message.Substring(0, Terminal.Width - 9);
                        message += "...";
                    }
                    Terminal.WriteLine(message);
                }
            }
            
            // Print a specific command.
            else {
                ThorCommand command;
                if (_commands.TryGetValue(commandName, out command)) {
                    Terminal.WriteLine("Usage:");
                    Terminal.WriteLine($" {name} {command.Example}");
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
        
        /// <summary>
        /// Invokes a command by name with the given arguments.
        /// </summary>
        /// <param name="commandName">The name of the command to invoke.</param>
        /// <param name="args">The arguments to provide to the command.</param>
        internal void Invoke(string commandName, string[] args) {
            // Show warnings for any public methods that don't have examples defined.
            foreach (string invalid in _commands.Where(p => p.Value.Example == null)
                                                .Select(p => p.Value.Name)) {
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
            
            var type = GetType().GetTypeInfo(); 
            
            // Find all public instance methods.  Ignore any public properties.
            return type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => typeof(Thor).GetTypeInfo().IsAssignableFrom(m.DeclaringType) &&
                                    !m.IsSpecialName)
                        .Select(m => new ThorCommand((IThor)this, new MethodInfoWrapper(m)))
                        .ToDictionary(c => c.Name);
        }
        
        /// <summary>
        /// Gets the first option provided by name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's value.</returns>
        protected string Option(string name, Func<string> defaultValue = null) {
            return Options(name).FirstOrDefault() ?? defaultValue?.Invoke();
        }
        
        /// <summary>
        /// Gets the first option provided by name converted to the type specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="convert">The delegate used for converting the text value to the type value.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's converted value.</returns>
        protected T Option<T>(string name, Func<string, T> convert = null, Func<T> defaultValue = null) {
            return Options<T>(name, convert, defaultValue).FirstOrDefault();
        }
        
        /// <summary>
        /// Gets all options provided by name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>The option's values.</returns>
        protected IEnumerable<string> Options(string name) {
            List<string> options;
            if (_options.TryGetValue(name, out options)) {
                return options;
            }
            else {
                return new string[0]; 
            }
        }
        
        /// <summary>
        /// Gets all options provided byn name converted to the type specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="convert">The delegate used for converting the text value to the type value.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's converted values.</returns>
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
        
        /// <summary>
        /// Starts the thor program.
        /// </summary>
        /// <param name="args">The arguments given from the command line.</param>
        public static void Start<T>(string[] args)
            where T : Thor, new() {

            string commandName;
            
            // Default to help.
            if (!args.Any()) {
                commandName = nameof(help);
            }
            else {
                commandName = args[0];
                args = args.Skip(1).ToArray();
            }
            
            T thor = new T();
            thor.Invoke(commandName, args);
        }
    }
}