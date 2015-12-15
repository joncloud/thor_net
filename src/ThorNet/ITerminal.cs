namespace ThorNet {
	/// <summary>
	/// Provides an interface with the terminal output.
	/// </summary>
	public interface ITerminal {
		
		/// <summary>
		/// Gets the total number of characters that can be printed on one line.
		/// </summary>
		int Width { get; }
		
		/// <summary>
		/// Prints the specified text to standard output.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void Write(string text);
		
		/// <summary>
		/// Prints a new line to the standard output.
		/// </summary>
		void WriteLine();
		
		/// <summary>
		/// Prints the specified text, and a new line to the standard output.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void WriteLine(string text);
	}
}