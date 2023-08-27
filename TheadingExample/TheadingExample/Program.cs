using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheadingExample
{
    internal class Program
    {
        private static readonly DateTime ProgramStart = DateTime.Now;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write(new StringBuilder()
                .AppendLine("1) Single Thread Example")
                .AppendLine("2) Multithreading Example")
                .AppendLine("3) Data Management")
                .AppendLine("0) Quit")
                .ToString());

                int choice;
                string input;
                do
                {
                    input = Console.ReadLine().Trim();
                } while (!int.TryParse(input, out choice));

                Console.WriteLine();
                switch (choice)
                {
                    case 0:
                        return;
                    case 1:
                        SingleThread();
                        break;
                    case 2:
                        MultiThread();
                        break;
                    case 3:
                        DataManagement();
                        break;
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Simluates work being done, marking when work is started and finished.
        /// </summary>
        private static void DoWork(string message = "Do Work", string id = null)
        {

            if (id != null)
            {
                message = $"{message} {id}";
            }

            Console.WriteLine(PreTime($"Start {message}"));
            Thread.Sleep(1000);
            Console.WriteLine(PreTime($"Finish {message}"));
        }

        /// <summary>
        /// <inheritdoc cref="DoWork(string, string)"/>
        /// </summary>
        private static void DoWork() { DoWork(id: null); }

        /// <summary>
        /// Goes through the concept of a thread using one instanced thread and the main thread.
        /// </summary>
        private static void SingleThread()
        {
            Thread thread;

            // ### Basic Threading Example ###
            // Creates and starts a new thread that will run at the same time as the main thread.
            // The child thread will do work for one second and the main thread will do work for 250ms 6 times.
            // Their work will appear to happen at the same time (synchronously) and the fourth main thread work will complete close to the child thread work.
            WriteHeader("Basic Example");

            // The Thread constructor can take a function as an input, and a Thread must be started with Start() to execute the function.
            thread = new Thread(DoWork);
            thread.Start();

            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(250);
                Console.WriteLine(PreTime("Main Thread Work"));
            }

            // Join makes the parent thread (the main thread) wait for the child thread to finish.
            thread.Join();

            // ### Join Example ###
            // Follows the same logic as "Basic Threading Example" except that the thread.join() call is placed after the start and before the main thread work is started.
            // Their work will happen one after another in this case. The child thread will run for one second AND THEN the main thread will run 6 times for 250ms.
            // This is the same as calling DoWork without the threading calls as they run one after another (asynchronously).
            WriteHeader("Join Example");

            thread = new Thread(DoWork);
            thread.Start();

            // Waits for the thread to finish
            thread.Join();

            // THEN does the main thread work
            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(250);
                Console.WriteLine(PreTime("Main Thread Work"));
            }
        }

        /// <summary>
        /// Introduces multithreading and the basic structure of running multipl threads.
        /// </summary>
        private static void MultiThread()
        {
            WriteHeader("Multithreading Example");

            // Create all threads
            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => { DoWork(id: System.Environment.CurrentManagedThreadId.ToString()); });
            }

            // Start all threads
            // Technically this can be done when you create the threads, but for consistency in the timestamps I put it here
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }

            // Wait for all threads to finish before continuing
            // If the result of all threads is needed, then this is required
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }

            Console.WriteLine("All Threads finished");
        }

        /// <summary>
        /// Shows examples of bad and good data usage with threads.
        /// Note the change in run times when as we can no longer run synchronously with the stored data.
        /// </summary>
        private static void DataManagement()
        {
            // Using up to date accurate data can be a big concern for some multithreading projects.
            // If a thread stores some data and the original data is updated, the thread is now working off of old data.
            // In this example the thread wants to modify and update the original source by adding 1.
            // Because it is working on old data the result will be incorrect.
            WriteHeader("Data Management - BAD");

            Random rnd = new Random();

            int badCount = 0;
            ThreadStart badThreadAction = () => {
                Thread.Sleep(rnd.Next(100));
                int storedCount = badCount;
                Thread.Sleep(rnd.Next(100));

                // There is a high chance "storedCount" is out of date with other threads running synchronously
                badCount = storedCount + 1;

                Console.WriteLine($"Thread #{System.Environment.CurrentManagedThreadId} Count: {badCount}");
            };

            // Run the threads
            Thread[] badThreads = new Thread[10];

            for (int i = 0; i < badThreads.Length; ++i)
            {
                badThreads[i] = new Thread(badThreadAction);
            }

            for (int i = 0; i < badThreads.Length; ++i)
            {
                badThreads[i].Start();
            }

            for (int i = 0; i < badThreads.Length; ++i)
            {
                badThreads[i].Join();
            }

            // Final count
            Console.WriteLine($"Final Bad Count: {badCount}");

            Console.WriteLine();

            // To work around data concurrency, one option is to "lock" the data and make it unable to be accessed until the lock is lifted.
            // One way to do that is using Mutexes - These can be locked and released and have methods that can wait for a mutex to be released before continuing execution.
            // This is just one way to handle this problem, there are many other ways with different effects on efficiency.
            WriteHeader("Data Management - GOOD");
            Mutex mut = new Mutex();

            int goodCount = 0;
            ThreadStart goodThreadAction = () =>
            {
                // Wait for the mutex to be unlocked once.
                // This also locks the mutex so that no other threads can pass.
                mut.WaitOne();

                // Perform the increment work... This is now "Thread Safe" as this area is protected by the mutex.
                Thread.Sleep(rnd.Next(100));
                int storedCount = goodCount;
                Thread.Sleep(rnd.Next(100));
                goodCount = storedCount + 1;

                Console.WriteLine($"Thread #{System.Environment.CurrentManagedThreadId} Count: {goodCount}");

                // Release the mutex once, signalling that the next thread may enter the protected area.
                mut.ReleaseMutex();
            };

            // Run the threads
            Thread[] goodThreads = new Thread[10];

            for (int i = 0; i < goodThreads.Length; ++i)
            {
                goodThreads[i] = new Thread(goodThreadAction);
            }

            for (int i = 0; i < goodThreads.Length; ++i)
            {
                goodThreads[i].Start();
            }

            for (int i = 0; i < goodThreads.Length; ++i)
            {
                goodThreads[i].Join();
            }

            // Final Count
            Console.WriteLine($"Final Good Count: {goodCount}");
        }

        private static void WriteHeader(String title)
        {
            int length = title.Length;

            string output = new StringBuilder()
                .Append('=', length + 8).Append("\n")
                .Append('=', 3).AppendFormat(" {0} ", title).Append('=', 3).Append("\n")
                .Append('=', length + 8).Append("\n")
                .ToString();

            Console.Write(output);
        }

        private static string PreTime(string message, int num = 1, int dec = 3)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan span = currentTime - ProgramStart;

            string timeFormat = new StringBuilder().Append('0', num).Append('.').Append('0', dec).ToString();
            return String.Format($"{{0:#,{timeFormat}}}: {{1}}", span.TotalSeconds, message);
        }
    }
}
