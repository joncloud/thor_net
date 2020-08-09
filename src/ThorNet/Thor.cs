using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThorNet
{
    /// <summary>
    /// Defines the core class for starting a thor program.
    /// </summary>
    public class Thor : IThor
    {
        internal readonly Dictionary<string, ThorCommand> Commands;
        private Dictionary<string, List<string>> _options;
        internal readonly Dictionary<string, Func<Thor>> SubCommands;

        /// <summary>
        /// Creates a new thor.
        /// </summary>
        public Thor()
            : this(new ConsoleWrapper())
        {
        }

        /// <summary>
        /// Creates a new thor with a terminal.
        /// </summary>
        /// <param name="terminal">The terminal to use to output responses to.</param>
        public Thor(ITerminal terminal)
        {
            Commands = LoadCommands();
            Terminal = terminal;
            _options = new Dictionary<string, List<string>>();
            SubCommands = new Dictionary<string, Func<Thor>>();
        }

        internal bool IsSubcommand { get; set; }

        /// <summary>
        /// Gets all options available to every method.
        /// </summary>
        IEnumerable<OptionAttribute> IThor.Options =>
            GetType().GetTypeInfo().GetCustomAttributes<OptionAttribute>();

        ITerminal Terminal { get; }

        /// <summary>
        /// Adds an option's value by name.
        /// </summary>
        /// <param name="name">The name of the option to provide a value for.</param>
        /// <param name="value">The value to provide.</param>
        void IThor.AddOption(string name, string value)
        {
            List<string> values;
            if (!_options.TryGetValue(name, out values))
            {
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
        protected bool Flag(string name) => Option(name) != null;

        /// <summary>
        /// Gets the name of the package for display in the <see cref="Help(string, string[])"/> method.
        /// </summary>
        protected virtual string GetPackageName() =>
            GetType().GetTypeInfo().Assembly.GetName().Name;

        /// <summary>
        /// Determines if the option has already been specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>True if there is a value for the option, otherwise false.</returns>
        bool IThor.HasOption(string name) => _options.ContainsKey(name);

        /// <summary>
        /// Prints help context for all commands exposed by this thor instance.
        /// </summary>
        /// <param name="commandName">The optional command name to provide detailed help for.</param>
        /// <param name="subcommandNames">The optional sub-command name(s) to provide detailed help for.</param>
        /// <returns>The exit code to return to the command line.</returns>
        [Alias("help")]
        [Desc("help COMMAND", "describe available commands or one specific command")]
        internal int Help(string commandName = null, string[] subcommandNames = null)
        {
            // Print all of the commands.
            if (commandName == null)
            {
                PrintSummaryHelp();
                return 0;
            }

            // Print a specific command.
            else
            {
                return PrintCommandHelp(commandName, subcommandNames);
            }
        }

        int HandleException(Exception ex)
        {
            Terminal.WriteLine($"[ERROR] {ex.Message}");
            return 1;
        }

        /// <summary>
        /// Invokes a command by name with the given arguments.
        /// </summary>
        /// <param name="commandName">The name of the command to invoke.</param>
        /// <param name="args">The arguments to provide to the command.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the exit code to return to the command prompt.</returns>
        internal async Task<int> InvokeAsync(string commandName, string[] args)
        {
            // Show warnings for any public methods that don't have examples defined.
            foreach (string invalid in Commands.Where(p => string.IsNullOrEmpty(p.Value.Example))
                                                .Select(p => p.Value.Display))
            {
                Terminal.WriteLine($"[WARNING] Attempted to create command \"{invalid}\" without usage or description. Add Desc if you want this method to be available as command, or declare it as a non-public member.");
            }

            ThorCommand command;
            if (Commands.TryGetValue(commandName, out command))
            {
                try { return await command.InvokeAsync(args); }
                catch (TargetInvocationException ex)
                {
                    return HandleException(ex.InnerException);
                }
                catch (Exception ex)
                {
                    return HandleException(ex);
                }
            }
            else
            {
                Thor subcommand;
                if (TryGetSubcommand(commandName, out subcommand))
                {
                    commandName = PrepareInvocationArguments(ref args);
                    return await subcommand.InvokeAsync(commandName, args);
                }

                else
                {
                    Terminal.WriteLine($"Could not find command \"{commandName}\".");
                    return 1;
                }
            }
        }

        private Dictionary<string, ThorCommand> LoadCommands()
        {
            var type = GetType().GetTypeInfo();

            // Find all public instance methods.  Ignore any public properties.
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => typeof(Thor).GetTypeInfo().IsAssignableFrom(m.DeclaringType))
                .Where(m => !m.IsSpecialName)
                .ToList();
            methods.Add(
                typeof(Thor)
                    .GetTypeInfo()
                    .GetMethod(nameof(Help), BindingFlags.Instance | BindingFlags.NonPublic)
            );

            var commands = new Dictionary<string, ThorCommand>();
            foreach (var method in methods)
            {
                var command = new ThorCommand(
                    this, 
                    new MethodInfoWrapper(method), 
                    Terminal
                );

                commands[command.Name] = command;
                if (command.HasAlias)
                {
                    commands[command.Alias] = command;
                }
            }

            return commands;
        }

        /// <summary>
        /// Gets the first option provided by name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's value.</returns>
        protected string Option(string name, Func<string> defaultValue = null) =>
            Options(name).FirstOrDefault() ?? defaultValue?.Invoke();

        /// <summary>
        /// Gets the first option provided by name converted to the type specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="convert">The delegate used for converting the text value to the type value.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's converted value.</returns>
        protected T Option<T>(string name, Func<string, T> convert = null, Func<T> defaultValue = null) =>
            Options(name, convert, defaultValue).FirstOrDefault();

        /// <summary>
        /// Gets all options provided by name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>The option's values.</returns>
        protected IEnumerable<string> Options(string name)
        {
            List<string> options;
            if (_options.TryGetValue(name, out options))
            {
                return options;
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// Gets all options provided by name converted to the type specified.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="convert">The delegate used for converting the text value to the type value.</param>
        /// <param name="defaultValue">The delegate used to provide a default value if no option is found.</param>
        /// <returns>The option's converted values.</returns>
        protected IEnumerable<T> Options<T>(string name, Func<string, T> convert = null, Func<T> defaultValue = null)
        {
            if (convert == null)
            {
                convert = s => TypeHelper.Convert<T>(s);
            }

            IEnumerable<string> options = Options(name);
            if (options.Any())
            {
                return options.Select(convert);
            }
            else
            {
                if (defaultValue == null)
                {
                    return new T[0];
                }
                else
                {
                    return new[] { defaultValue() };
                }
            }
        }

        /// <summary>
        /// Prepares the input arguments to invoke <see cref="Thor.InvokeAsync(string, string[])"/>
        /// </summary>
        /// <param name="args">The array of arguments provided.  This is modified to remove the first argument if present.</param>
        /// <returns>The name of the command to invoke.  If no arguments are provided, this defaults to <see cref="Help(string, string[])"/>.</returns>
        internal static string PrepareInvocationArguments(ref string[] args)
        {
            string commandName;

            // Default to help.
            if (!args.Any())
            {
                commandName = nameof(Help);
            }
            else
            {
                commandName = args[0];
                args = args.Skip(1).ToArray();
            }

            return commandName;
        }

        IEnumerable<Thor> GetAllSubCommands(Thor parent)
        {
            foreach (var factory in parent.SubCommands.Values)
            {
                var child = factory();
                yield return child;

                foreach (var subFactory in GetAllSubCommands(child))
                {
                    yield return subFactory;
                }
            }
        }

        IEnumerable<string> GetLongDescriptionLines(string description)
        {
            var lines = description.Split(new[] { "\r\n" }, StringSplitOptions.None);

            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) && sb.Length > 0)
                {
                    Terminal.Wrap(sb);
                    sb.AppendLine();
                    yield return sb.ToString();
                    sb.Clear();
                }

                else
                {
                    string trimmed = line.Trim();
                    if (trimmed.Length > 0 && trimmed[0] == '\x5')
                    {
                        sb.AppendLine()
                            .Append(trimmed.Substring(1));
                    }
                    else
                    {
                        if (sb.Length > 0) { sb.Append(" "); }
                        sb.Append(trimmed);
                    }
                }
            }

            if (sb.Length > 0)
            {
                Terminal.Wrap(sb);
                sb.AppendLine();
                yield return sb.ToString();
                sb.Clear();
            }
        }

        int PrintCommandHelp(string commandName, string[] subcommandNames)
        {
            // Handle commands.
            ThorCommand command;
            if (Commands.TryGetValue(commandName, out command))
            {
                Terminal.WriteLine("Usage:");
                string name = GetPackageName();
                Terminal.WriteLine($"  {name} {command.Example}");
                Terminal.WriteLine();

                // Print the options.
                OptionAttribute[] options = command.Options
                    .OrderBy(o => o.Alias)
                    .ThenBy(o => o.Name)
                    .ToArray();
                if (options.Any())
                {
                    Terminal.WriteLine("Options:");

                    const string prefix = "  ";
                    const string separator = ", ";
                    const string namePrefix = "--";
                    const string nameSeparator = "=";
                    const string openBracket = "[";
                    const string closeBracket = "]";
                    int max = options.Max(o =>
                        prefix.Length
                            + o.Alias.Length + (o.Flag && o.Alias[0] != '-' ? 1 : 0)
                            + o.Name.Length
                            + (o.Flag ? nameSeparator.Length + o.Name.Length : 0))
                        + separator.Length
                        + openBracket.Length
                        + namePrefix.Length
                        + closeBracket.Length;

                    StringBuilder message = new StringBuilder();

                    foreach (OptionAttribute option in options)
                    {
                        message.Append(prefix);

                        if (option.Alias[0] != '-')
                        {
                            message.Append("-");
                        }

                        message.Append(option.Alias)
                            .Append(separator)
                            .Append(openBracket)
                            .Append(namePrefix)
                            .Append(option.Name);

                        if (!option.Flag)
                        {
                            message.Append(nameSeparator)
                                .Append(option.Name.ToUpper());
                        }

                        message.Append(closeBracket);

                        if (message.Length < max)
                        {
                            int delta = max - message.Length;
                            message.Append(' ', delta);
                        }

                        message.Append(" # ")
                            .Append(option.Description);

                        Terminal.Truncate(message);

                        Terminal.WriteLine(message.ToString());
                        message.Clear();
                    }
                    Terminal.WriteLine();
                }

                Terminal.WriteLine(command.Description);

                if (!string.IsNullOrEmpty(command.LongDescription))
                {
                    Terminal.WriteLine();
                    foreach (string line in GetLongDescriptionLines(command.LongDescription))
                    {
                        Terminal.WriteLine(line);
                    }
                }
            }
            else
            {
                // Handle subcommands.
                Thor subcommand;
                if (TryGetSubcommand(commandName, out subcommand))
                {
                    return subcommand.Help(
                        subcommandNames?.FirstOrDefault(), 
                        subcommandNames?.Skip(1).ToArray());
                }
                else
                {
                    Terminal.WriteLine($"Could not find command \"{commandName}\".");
                    return 1;
                }
            }

            return 0;
        }

        void PrintSummaryHelp()
        {
            Terminal.WriteLine("Tasks:");

            var commands = GetAllSubCommands(this)
                .SelectMany(t => t.Commands)
                .Where(pair => pair.Key != nameof(Help))
                .Concat(Commands)
                .Select(p => p.Value)
                .Distinct()
                .ToArray();

            const string prefix = "  ";
            int maxExample = commands.Max(c => c.Example.Length) + prefix.Length;

            StringBuilder message = new StringBuilder();

            foreach (ThorCommand command in commands.OrderBy(c => c.Example))
            {
                if (IsSubcommand && command.Name == nameof(Help)) { continue; }

                // Create a message like '  {example}  # {description}'
                message.Append(prefix)
                    .Append(command.Example);

                if (message.Length < maxExample)
                {
                    int delta = maxExample - message.Length;
                    message.Append(' ', delta);
                }

                message.Append(" # ")
                    .Append(command.Description);

                Terminal.Truncate(message);

                Terminal.WriteLine(message.ToString());
                message.Clear();
            }
        }

        /// <summary>
        /// Starts the thor program.
        /// </summary>
        /// <param name="args">The arguments given from the command line.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the exit code to return to the command prompt.</returns>
        public static async Task<int> StartAsync<T>(string[] args)
            where T : Thor, new()
        {
            string commandName = PrepareInvocationArguments(ref args);

            T thor = new T();
            return await thor.InvokeAsync(commandName, args);
        }

        /// <summary>
        /// Associates a subcommand with this command.
        /// </summary>
        /// <typeparam name="T">The type of class to use for a subcommand.</typeparam>
        /// <param name="name">The optional name of the subcommand.  If no name is provided, then the name of the class is used.</param>
        protected void Subcommand<T>(string name = null)
            where T : Thor, new()
        {
            name = name ?? typeof(T).Name;
            if (Commands.ContainsKey(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name), $"{name} is a command, and cannot also be a subcommand.");
            }

            SubCommands.Add(name ?? typeof(T).Name, () => new T() { IsSubcommand = true });
        }

        bool TryGetSubcommand(string name, out Thor thor)
        {
            Func<Thor> factory;
            if (SubCommands.TryGetValue(name, out factory))
            {
                thor = factory();
                return true;
            }
            else
            {
                thor = null;
                return false;
            }
        }
    }
}