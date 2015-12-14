using System;

namespace ThorNet {
	
	/// <summary>
	/// Provides an interface to the main thor class.
	/// </summary>
	public interface IThor {
		
		/// <summary>
		/// Adds an option's value by name.
		/// </summary>
		/// <param name="name">The name of the option to provide a value for.</param>
		/// <param name="value">The value to provide.</param>
		void AddOption(string name, string value);
		
		/// <summary>
		/// Determines if the option has already been specified.
		/// </summary>
		/// <param name="name">The name of the option.</param>
		/// <returns>True if there is a value for the option, otherwise false.</returns>
		bool HasOption(string name);
	}
}