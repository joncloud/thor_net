using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThorNet.Terminal
{
    public class Program : Thor {
        public static void Main(string[] args) {
            Start<Program>(args);
        }
        
        [Desc("roll EXPR", "rolls dice using D&D notation, i.e., 2d6.")]
        public void roll(string expr) {
            Dice dice;
            if (Dice.TryParse(expr, out dice)) {
                Console.WriteLine($"Rolling {expr}");
                int[] results = dice.Roll();
                foreach (int result in results) {
                    Console.WriteLine($"\t{result}");
                }
            } 
            else {
                Console.WriteLine("Invalid dice expression.");
            }
        }
        
        [Desc("roll EXPR", "rolls dice using D&D notation, i.e., 2d6 asynchronously.")]
        public Task rollAsync(string expr) {
            return Task.Run(() => roll(expr));
        }
    }
    
    public class Dice {
        
        private Random _random = new Random();
        
        public Dice(int sides)
            : this(sides, 1) {
            
        }
        
        public Dice(int sides, int count) {
            if (sides <= 0) throw new ArgumentOutOfRangeException(nameof(sides));
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            
            Count = count;
            Sides = sides;
        }
        
        public int Count { get; }
        public int Sides { get; }
        
        public int[] Roll() {
            int[] results = new int[Count];
            for (int i = 0; i < results.Length; i++) {
                results[i] = _random.Next(1, Sides + 1);
            }
            return results;
        }
        
        public static bool TryParse(string text, out Dice dice) {
            
            dice = null;
            
            string[] parts = text.Split(new [] { 'd' }, StringSplitOptions.RemoveEmptyEntries);
            int[] values = new int[parts.Length];
            
            for (int i = 0; i < parts.Length; i++) {
                if (!int.TryParse(parts[i], out values[i])) {
                    return false;
                }
            }
            
            switch (parts.Length) {
                case 1:
                    dice = new Dice(values[0]);
                    return true;
                    break;
                case 2:
                    dice = new Dice(values[1], values[0]);
                    return true;
                default:
                    return false;
            }
        }
    }
}
