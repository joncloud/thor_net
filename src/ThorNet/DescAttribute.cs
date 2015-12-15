using System;

namespace ThorNet {
    /// <summary>
    /// Describes a command.
    /// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DescAttribute : Attribute {
		public DescAttribute(string example, string description) {
			Description = description;
			Example = example;
		}
		
        /// <summary>
        /// Gets the description for the command.
        /// </summary>
		public string Description { get; }
        
        /// <summary>
        /// Gets the brief example of calling the command.
        /// </summary>
		public string Example { get; }
	}
}