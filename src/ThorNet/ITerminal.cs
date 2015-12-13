namespace ThorNet {
	public interface ITerminal {
		int Width { get; }
		
		void Write(string text);
		void WriteLine();
		void WriteLine(string text);
	}
}