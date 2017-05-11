namespace ThorNet.UnitTests
{
    public class NullTerminal : ITerminal
    {
        public static NullTerminal Instance { get; } = new NullTerminal();

        public int Width => 100;

        public void Write(string text) { }

        public void WriteLine() { }

        public void WriteLine(string text) { }
    }
}
