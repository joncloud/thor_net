using System;
using System.Text;

namespace ThorNet.Sample
{
    public class Program : Thor
    {
        public Program()
        {
            Subcommand<Messages>("messages");
        }

        [Desc("count TO", "count up to TO")]
        public void Count(int to)
        {
            for (int i = 1; i <= to; i++)
            {
                Console.WriteLine(i);
            }
        }

        [Desc("Hello NAME", "say hello to NAME")]
        [MethodOption("from", "f", "who the message is from")]
        [MethodOption("repeat", "r", "repeats the message")]
        [MethodOption("yell", "y", "yells the message", Flag = true)]
        public void Hello(string name)
        {
            StringBuilder output = new StringBuilder();

            string from = Option("from");
            if (from != null) { output.AppendLine($"From: {from}"); }
            output.AppendLine($"Hello {name}");

            int repeats = Option("repeat", defaultValue: () => 1);
            while (--repeats >= 0)
            {
                Console.Write(Flag("yell") ? output.ToString().ToUpper() : output.ToString());
            }
        }

        public static void Main(string[] args)
        {
            Start<Program>(args);
        }
    }
}
