using System;
using System.Text;

namespace ThorNet
{
    internal static class TerminalExtensions
    {
        public static void Truncate(this ITerminal terminal, StringBuilder sb)
        {
            const string suffix = "\b...";
            if (sb.Length > terminal.Width)
            {
                int delta = sb.Length - terminal.Width + suffix.Length;
                sb.Remove(terminal.Width - suffix.Length, delta);
                sb.Append(suffix);
            }
        }

        public static void Wrap(this ITerminal terminal, StringBuilder sb)
        {
            int length = sb.Length, count = 0;
            while (length > terminal.Width)
            {
                sb.Insert((terminal.Width * ++count), Environment.NewLine);
                length -= terminal.Width;
            }
        }
    }
}
