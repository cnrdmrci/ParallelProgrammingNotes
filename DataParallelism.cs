//-----Data Parallelism-----

//1- Parallel For -----------------------

//A- Normal For -------
for (int i = 0; i < 10; i++)
{
    long total = GetTotal();
    Console.WriteLine("{0} - {1}", i, total);
}


//B- Parallel For -------
Parallel.For(0, 10, i =>
{
    long total = GetTotal();
    Console.WriteLine("{0} - {1}", i, total);
});
//---
var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount) };
Parallel.For(0, 10,parallelOptions, i =>
{
    long total = GetTotal();
    Console.WriteLine("{0} - {1}", i, total);
});


//C- Parallel For with Break Example	-------
Parallel.For(1, 20, (i, pls) =>
{
    Console.Write(i + " ");
    if (i >= 15)
    {
        Console.WriteLine("Break on {0}", i);
        pls.Break();
    }
});


//D- Parallel For more detailed
int Foo()
{
    int total = 0;
    Parallel.For(1, 10001, 
        () => 0, // initial value,
        (num, state, localSum) => num + localSum,
        localSum => Interlocked.Add(ref total, localSum));
    return total; // total = 50005000
}

//2- Parallel ForEach -----------------------

//A- Normal ForEach -------
foreach (int i in Enumerable.Range(1, 10))
{
    Console.WriteLine("{0} - {1}", i, GetTotal());
}

//B- Parallel ForEach -------
Parallel.ForEach(Enumerable.Range(1, 10), i => 
{
    Console.WriteLine("{0} - {1}", i, GetTotal());
});

//C- Parallel Safe ForEach -------
object sync = new object();
long total = 0;
 
Parallel.ForEach(Enumerable.Range(1, 100000000), value =>
{
    lock (sync)
    {
        total += value;
    }
});


//D- Parallel ForEach more detailed
int Foo()
{
    int total = 0;
    var numbers = Enumerable.Range(1, 10000).ToList();
    Parallel.ForEach(numbers, 
        () => 0, // initial value,
        (num, state, localSum) => num + localSum,
        localSum => Interlocked.Add(ref total, localSum));
    return total; // total = 50005000
}

//E- Parallel ForEach vs Plinq

Enumerable.Range(1, 10).AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll((i)=> functionName(i));

Enumerable.Range(1, 10).AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .ForAll(functionName);


//3- Parallel For full example -------------------


using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

class Program
{
    static int[] _values = Enumerable.Range(0, 1000).ToArray();

    static void Example(int x)
    {
        // Sum all the elements in the array.
        int sum = 0;
        int product = 1;
        foreach (var element in _values)
        {
            sum += element;
            product *= element;
        }
    }

    static void Main()
    {
        const int max = 10;
        const int inner = 100000;
        var s1 = Stopwatch.StartNew();
        for (int i = 0; i < max; i++)
        {
            Parallel.For(0, inner, Example);
        }
        s1.Stop();
        var s2 = Stopwatch.StartNew();
        for (int i = 0; i < max; i++)
        {
            for (int z = 0; z < inner; z++)
            {
                Example(z);
            }
        }
        s2.Stop();
        Console.WriteLine(((double)(s1.Elapsed.TotalMilliseconds * 1000000) /
            max).ToString("0.00 ns"));
        Console.WriteLine(((double)(s2.Elapsed.TotalMilliseconds * 1000000) /
            max).ToString("0.00 ns"));
        Console.Read();
    }
}

//Results

//11229670.00 ns Parallel.For
//46920150.00 ns for


//4- Parallel Invoke --------------------


using System;
using System.Threading.Tasks;

class Program
{
    static void Test()
    {
        // This method always finishes last, because the methods are run in parallel.
        // ... And this one has some computations in it that slow it down.
        for (int i = 0; i < 1000000; i++)
        {
            double value = Math.Sqrt(i);
            if (value == -1)
            {
                return;
            }
        }
        Console.WriteLine("Test");
    }

    static void Test2()
    {
        Console.WriteLine("Test2");
    }

    static void Test3()
    {
        Console.WriteLine("Test3");
    }

    static void Main()
    {
        Parallel.Invoke(Test, Test2, Test3);
    }
}

//Output

//Test2
//Test3
//Test


//--example
List<Action> actionsList = new List<Action>
            {
                () => Console.WriteLine("0"),
                () => Console.WriteLine("1"),
                () => Console.WriteLine("2"),
                () => Console.WriteLine("3"),
                () => Console.WriteLine("4"),
                () => Console.WriteLine("5"),
                () => Console.WriteLine("6"),
                () => Console.WriteLine("7"),
                () => Console.WriteLine("8"),
                () => Console.WriteLine("9"),
            };

            Parallel.ForEach<Action>(actionsList, ( o => o() ));

            Console.WriteLine();

            Action[] actionsArray = new Action[]
            {
                () => Console.WriteLine("0"),
                () => Console.WriteLine("1"),
                () => Console.WriteLine("2"),
                () => Console.WriteLine("3"),
                () => Console.WriteLine("4"),
                () => Console.WriteLine("5"),
                () => Console.WriteLine("6"),
                () => Console.WriteLine("7"),
                () => Console.WriteLine("8"),
                () => Console.WriteLine("9"),
            };

            Parallel.Invoke(actionsArray);

            Console.ReadKey();