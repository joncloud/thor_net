using System;

namespace ThorNet
{
    /// <summary>
    /// Defines an available option for a command.
    /// </summary> 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class OptionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new option attribute.
        /// </summary>
        public OptionAttribute(string name, string alias, string description)
        {
            Alias = alias;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Gets the alias to use for short-hand calls to the option.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets or sets the default value for the option if no option is specified.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets the description of the option.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets whether or not this option should be treated as a flag.
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        public string Name { get; }

        Type _enumType;
        /// <summary>
        /// Gets or sets the enum type used to help describe information in help.
        /// </summary>
        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (!(value is null) && !value.IsEnum)
                {
                    throw new ArgumentException("value must be an enum type", nameof(value));
                }
                _enumType = value;
            }
        }

        internal string[] GetPossibleValues()
        {
            if (!(EnumType is null))
            {
                return Enum.GetNames(EnumType);
            }

            return null;
        }
    }
}