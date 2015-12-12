using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThorNet.Terminal
{
    public class Program {
        public static void Main(string[] args) {
            Thor.Start<MyThor>(args);
        }
    }
    
    public class MyThor : Thor {
        [Desc("roll SIDES COUNT", "rolls dice")]
        [MethodOption("count", "-c", "The total number of times to roll the dice.")]
        [MethodOption("sides", "-s", "The total number of sides on the dice to roll.")]
        public void roll(int sides, int count = 1) {
            Console.WriteLine($"Rolling {count}d{sides}");
            Random random = new Random();
            while (--count >= 0) {
                int value = random.Next(1, sides + 1);
                Console.WriteLine($"1d{sides}\t{value}");
            }
        }
        
        [Desc("rollAsync SIDES COUNT", "rolls dice asynchronously")]
        [MethodOption("count", "-c", "The total number of times to roll the dice.")]
        [MethodOption("sides", "-s", "The total number of sides on the dice to roll.")]
        public Task rollAsync(int sides, int count = 1) {
            return Task.Run(() => roll(sides, count));
        }
    }
}
