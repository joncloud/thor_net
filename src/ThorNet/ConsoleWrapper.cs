using System;

namespace ThorNet {
	
	public class ConsoleWrapper : ITerminal {
		public int Width { get { return Console.WindowWidth; } }
		
		public void Write(string text) {
			Console.Write(text);
		}
		
		public void WriteLine() {
			Console.WriteLine();
		}
		
		public void WriteLine(string text) {
			Console.WriteLine(text);
		}
	}
	
}