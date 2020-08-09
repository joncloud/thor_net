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

        [Alias("count")]
        [Desc("count TO", "count up to TO")]
        public void Count(int to)
        {
            for (int i = 1; i <= to; i++)
            {
                Console.WriteLine(i);
            }
        }

        [Alias("delay")]
        [Desc("delay TIME", "delays in milliseconds")]
        public async Task Delay(int time)
        {
            bool verbose = Flag("verbose");

            if (verbose) { Console.WriteLine($"> delaying {time}ms"); }
            await Task.Delay(time);
            if (verbose) { Console.WriteLine($"> done delaying {time}ms"); }
        }

        [Alias("exit")]
        [Desc("exit CODE", "stops the process with the specified exit code")]
        public int Exit(int code) => code;

        [Alias("goodbye")]
        [Desc("goodbye", "say goodbye to the world")]
        public void Goodbye()
        {
            bool verbose = Flag("verbose");

            if (verbose) { Console.WriteLine("> saying goodbye"); }
            Console.WriteLine("Goodbye World");
            if (verbose) { Console.WriteLine("> done saying goodbye"); }
        }

        [Alias("hello")]
        [Desc("hello NAME", "say hello to NAME")]
        [LongDesc(@"
            `ThorNet.Sample Hello` will print out a message to a person of your
            choosing.
 
            You can optionally specify a second parameter, which will print
            out a from message as well.
 
            PS C:\MyCLI> ThorNet.Sample hello Jonathan --from=Thor
            
            From: Thor
            
            Hello Jonathan")]
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

        public static Task<int> Main(string[] args)
        {
            return StartAsync<Program>(args);
        }
    }
}
