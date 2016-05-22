using System;
using System.Text;
using System.Threading.Tasks;

namespace ThorNet.Sample
{
    [Option("verbose", "v", "prints debugging information", Flag = true)]
    public class Program : Thor
    {
        public Program()
        {
            Subcommand<Messages>("messages");
        }

        [Desc("Count TO", "count up to TO")]
        public void Count(int to)
        {
            for (int i = 1; i <= to; i++)
            {
                Console.WriteLine(i);
            }
        }

        [Desc("Delay TIME", "delays in milliseconds")]
        public async Task Delay(int time)
        {
            bool verbose = Flag("verbose");

            if (verbose) { Console.WriteLine($"> delaying {time}ms"); }
            await Task.Delay(time);
            if (verbose) { Console.WriteLine($"> done delaying {time}ms"); }
        }
        
        [Desc("Goodbye", "say goodbye to the world")]
        public void Goodbye()
        {
            bool verbose = Flag("verbose");

            if (verbose) { Console.WriteLine("> saying goodbye"); }
            Console.WriteLine("Goodbye World");
            if (verbose) { Console.WriteLine("> done saying goodbye"); }
        }

        [Desc("Hello NAME", "say hello to NAME")]
        [Option("from", "f", "who the message is from")]
        [Option("repeat", "r", "repeats the message")]
        [Option("yell", "y", "yells the message", Flag = true)]
        public void Hello(string name)
        {
            bool verbose = Flag("verbose");
            StringBuilder output = new StringBuilder();

            if (verbose) { Console.WriteLine("> saying hello"); }

            string from = Option("from");
            if (from != null) { output.AppendLine($"From: {from}"); }
            output.AppendLine($"Hello {name}");

            int repeats = Option("repeat", defaultValue: () => 1);
            if (verbose) { Console.WriteLine($"> repeating {repeats}"); }
            while (--repeats >= 0)
            {
                Console.Write(Flag("yell") ? output.ToString().ToUpper() : output.ToString());
            }

            if (verbose) { Console.WriteLine("> done saying hello"); }
        }

        public static void Main(string[] args)
        {
            Start<Program>(args);
        }
    }
}
