using System;

namespace ThorNet {
	/// <summary>
	/// Defines an available option for a command.
	/// </summary> 
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MethodOptionAttribute : Attribute {
		public MethodOptionAttribute(string name, string alias, string description) {
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
	}
}