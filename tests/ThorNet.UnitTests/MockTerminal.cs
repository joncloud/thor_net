using System;
using System.Collections.Generic;
using System.Text;

namespace ThorNet.UnitTests
{
    public class MockTerminal : ITerminal
    {
        readonly StringBuilder _sb;
        public int Width { get; }
        public MockTerminal(int width)
        {
            Width = width;
            _sb = new StringBuilder();
        }

        public void Write(string text) =>
            _sb.Append(text);

        public void WriteLine() =>
            _sb.AppendLine();

        public void WriteLine(string text) =>
            _sb.AppendLine(text);

        public IEnumerable<string> GetLines() =>
            ToString()
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        public override string ToString() =>
            _sb.ToString();
    }
}
