//C# program that benchmarks SetMinThreads

using System;
using System.Threading;

class Program
{
    static void Main()
    {
        // Loop through number of min threads we use
        for (int c = 2; c <= 40; c++)
        {
            // Use AutoResetEvent for thread management
            AutoResetEvent[] arr = new AutoResetEvent[50];
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = new AutoResetEvent(false);
            }

            // Set the number of minimum threads
            ThreadPool.SetMinThreads(c, 4);

            // Get current time
            long t1 = Environment.TickCount;

            // Enqueue 50 work items that run the code in this delegate function
            for (int i = 0; i < arr.Length; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate(object o)
                {
                    Thread.Sleep(100);
                    arr[(int)o].Set(); // Signals completion

                }, i);
            }

            // Wait for all tasks to complete
            WaitHandle.WaitAll(arr);

            // Write benchmark results
            long t2 = Environment.TickCount;
            Console.WriteLine("{0},{1}",
                c,
                t2 - t1);
        }
    }
}