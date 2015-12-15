using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThorNet.Terminal
{
    public class Program : Thor {

        protected override string GetPackageName() {
            return "dice";
        }

        public static void Main(string[] args) {
            Start<Program>(args);
        }
        
        [Desc("roll EXPR", "rolls dice using D&D notation, i.e., 2d6.")]
        public void roll(string expr) {
            Dice dice;
            if (Dice.TryParse(expr, out dice)) {
                Console.WriteLine($"Rolling {expr}");
                int[] results = dice.Roll();
                int total = 0;
                foreach (int result in results) {
                    Console.WriteLine($"\t{result}");
                    total += result;
                }

                string text = total.ToString();
                Console.Write("\t");
                Console.WriteLine(new string('-', text.Length));
                Console.Write("\t");
                Console.WriteLine(text);
            } 
            else {
                Console.WriteLine("Invalid dice expression.");
            }
        }
        
        [Desc("rollAsync EXPR", "rolls dice using D&D notation, i.e., 2d6 asynchronously.")]
        public Task rollAsync(string expr) {
            return Task.Run(() => roll(expr));
        }
    }
}
