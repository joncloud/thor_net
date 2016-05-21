using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThorNet
{
    public static class TerminalExtensions
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
    }
}
