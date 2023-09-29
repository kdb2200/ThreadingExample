using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Runner().Wait();

            Console.WriteLine("\nPRESS ANY KEY TO EXIT");
            Console.ReadKey();
        }

        public static async Task Runner()
        {
            Task parallelTask = ParallelFunction();

            Console.WriteLine("I'M DOING OTHER STUFF IN PARALLEL");

            await parallelTask;
        }

        public static async Task ParallelFunction()
        {
            Console.WriteLine("ParallelFunction Started");

            await Task.Run(() => { Thread.Sleep(1000); });

            Console.WriteLine("ParallelFunction Finished");
        }
    }
}
