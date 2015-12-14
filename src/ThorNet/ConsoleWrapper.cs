using System;

namespace ThorNet {
	/// <summary>
	/// Implements a terminal by wrapping <see cref="Console" />
	/// </summary>
	public class ConsoleWrapper : ITerminal {
		/// <summary>
		/// Gets the total number of characters that can be printed on one line.
		/// </summary>
		public int Width { get { return Console.WindowWidth; } }
		
		/// <summary>
		/// Prints the specified text to standard output.
		/// </summary>
		/// <param name="text">The text to print.</param>
		public void Write(string text) {
			Console.Write(text);
		}
		
		/// <summary>
		/// Prints a new line to the standard output.
		/// </summary>
		public void WriteLine() {
			Console.WriteLine();
		}
		
		/// <summary>
		/// Prints the specified text, and a new line to the standard output.
		/// </summary>
		/// <param name="text">The text to print.</param>
		public void WriteLine(string text) {
			Console.WriteLine(text);
		}
	}
	
}