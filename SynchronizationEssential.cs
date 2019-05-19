//-----Synchronization Essential------

//1- Simple blocking methods
//These wait for another thread to finish or for a period of time to elapse. 
//Sleep, Join, and Task.Wait are simple blocking methods.

//2- Locking constructs
//These limit the number of threads that can perform some activity or execute a section of code at a time. 
//Exclusive locking constructs are most common — these allow just one thread in at a time, 
//and allow competing threads to access common data without interfering 
//with each other. The standard exclusive locking constructs are lock (Monitor.Enter/Monitor.Exit), Mutex, and SpinLock. 
//The nonexclusive locking constructs are Semaphore, SemaphoreSlim, and the reader/writer locks.

//3- Signaling constructs
//These allow a thread to pause until receiving a notification from another, avoiding the need for inefficient polling. 
//There are two commonly used signaling devices: event wait handles and Monitor’s Wait/Pulse methods. 
//Framework 4.0 introduces the CountdownEvent and Barrier classes.

//4- Nonblocking synchronization constructs
//These protect access to a common field by calling upon processor primitives. 
//The CLR and C# provide the following nonblocking constructs: 
//Thread.MemoryBarrier, Thread.VolatileRead, Thread.VolatileWrite, the volatile keyword, and the Interlocked class.

//---------Locking
static readonly object _locker = new object();
lock (_locker){};

//---------

//---------Deadlocks
object locker1 = new object();
object locker2 = new object();
 
new Thread (() => {
                    lock (locker1)
                    {
                      Thread.Sleep (1000);
                      lock (locker2);      // Deadlock
                    }
                  }).Start();
lock (locker2)
{
  Thread.Sleep (1000);
  lock (locker1);                          // Deadlock
}

//----Performance
Locking is fast
Acquiring and releasing an uncontended Mutex takes a few microseconds — about 50 times slower than a lock.



//----Mutex
private readonly Mutex m = new Mutex();
public void ThreadSafeMethod() {
    m.WaitOne();
    try {
        /* critical code */
    } finally {
        m.ReleaseMutex();
    }
}


//----Semaphore
class TheClub      // No door lists!
{
  static SemaphoreSlim _sem = new SemaphoreSlim (3);    // Capacity of 3
 
  static void Main()
  {
    for (int i = 1; i <= 5; i++) new Thread (Enter).Start (i);
  }
 
  static void Enter (object id)
  {
    Console.WriteLine (id + " wants to enter");
    _sem.Wait();
    Console.WriteLine (id + " is in!");           // Only three threads
    Thread.Sleep (1000 * (int) id);               // can be here at
    Console.WriteLine (id + " is leaving");       // a time.
    _sem.Release();
  }
}
**output :
1 wants to enter
1 is in!
2 wants to enter
2 is in!
3 wants to enter
3 is in!
4 wants to enter
5 wants to enter
1 is leaving
4 is in!
2 is leaving
5 is in!



//----AutoResetEvent
class BasicWaitHandle
{
  EventWaitHandle _waitHandle = new AutoResetEvent (false);
 
  void Main()
  {
    new Thread (Waiter).Start();
    Thread.Sleep (1000);                  // Pause for a second...
    _waitHandle.Set();                    // Wake up the Waiter.
  }
 
  void Waiter()
  {
    Console.WriteLine ("Waiting...");
    _waitHandle.WaitOne();                // Wait for notification
    Console.WriteLine ("Notified");
  }
}




//-----ManualResetEvent
As with AutoResetEvent, you can construct a ManualResetEvent in two ways:

var manual1 = new ManualResetEvent (false);
var manual2 = new EventWaitHandle (false, EventResetMode.ManualReset);



//-----CountdownEvent
CountdownEvent _countdown = new CountdownEvent (3);
 
void Main()
{
  new Thread (SaySomething).Start ("I am thread 1");
  new Thread (SaySomething).Start ("I am thread 2");
  new Thread (SaySomething).Start ("I am thread 3");
 
  _countdown.Wait();   // Blocks until Signal has been called 3 times
  Console.WriteLine ("All threads have finished speaking!");
}
 
void SaySomething (object thing)
{
  Thread.Sleep (1000);
  Console.WriteLine (thing);
  _countdown.Signal();
}


//----WaitAny, WaitAll, WhenAll, ContinueWith
//WaitAny herhangi bir task tamamlandiginda bloklama durur program devam eder.

//The TPL (Task Parallel Library) is one of the most interesting new features added in the recent versions of .Net framework. 
//The Task.WaitAll and Task.WhenAll methods are two important methods (and also frequently used) in the TPL.

//The Task.WaitAll blocks the current thread until all other tasks have completed execution. 
//The Task.WhenAll method is used to create a task that will complete if and only if all the other tasks are complete.

//So, if you are using Task.WhenAll you would get a task object that isn't complete. 
//However, it will not block and would allow the program to execute. 
//On the contrary, the Task.WaitAll method call actually blocks and waits for all other tasks to complete.

//ContinueWith -> thread başlatıldığında ve ana thread çalışmaya devam ederken, 
//işlem gören thradimiz bittiği anda devam edecek işlemlerin temsil ediliği fonksiyon


//Thread.Sleep() vs Thread.SpinWait()

//SpinWait() method’u, çağıran thread’i, işlemci üzerinde aktif tutar, 
//Sleep() method’u ise, thread’i belirli süre boyunca gerçek anlamda uyutur.




//WhenAll and ContinueWith kullanimi
Task<string> task1 = Task<string>.Factory.StartNew(() =>
{
    Thread.Sleep(1000);
    Console.WriteLine("task1 basladi");
    return "task1 bitti";
});

Task<string> task2 = Task<string>.Factory.StartNew(() =>
{
    Thread.Sleep(2000);
    Console.WriteLine("task2 basladi");
    return "task2 bitti";
});

Task<string> task3 = Task<string>.Factory.StartNew(() =>
{
    Thread.Sleep(3000);
    Console.WriteLine("task3 basladi");
    return "task3 bitti";
});

var sonuc = Task.WhenAll(task1, task2, task3);

Console.WriteLine("ana thread");

sonuc.ContinueWith((x) => {
    Console.WriteLine(task1.Result);
    Console.WriteLine(task2.Result);
    Console.WriteLine(task3.Result);
});

Console.WriteLine("Finish!");




//----The Barrier Class
Barrier _barrier = new Barrier (3);
 //bariyer sayisi 3 iken bariyere gelen iş parçacığı 3'e ulaştığında otomatik hepsi salınır ve tekrar aynı işlem uygulanır.
void Main()
{
  new Thread (Speak).Start();
  new Thread (Speak).Start();
  new Thread (Speak).Start();
}
 
void Speak()
{
  for (int i = 0; i < 5; i++)
  {
    Console.Write (i + " ");
    _barrier.SignalAndWait();
  }
}



//------ConcurrentBag<>
//The biggest advantage here is that ConcurrentBag<T> is safe to access from multiple threads while LisT<T> is not. 
//If thread safe access is important for your scenario then a type like ConcurrentBag<T> is possibly to your advantage over List<T> + manual locking. 
//We'd need to know a bit more about your scenario before we can really answer this question.

//Additionally List<T> is an ordered collection while ConcurrentBag<T> is not.

//---
//Add(T element) - This method is used to add an element to the ConcurrentBag<T>.
//TryPeek(out T) - This method is used to retrieve an element from ConcurrentBag<T> without removing it.
//TryTake(out T) - This method is used to retrieve an element from ConcurrentBag<T>. Note that this method removes the item from the collection.

//ConcurrentBag approximately two times slower than lock.

//------ConcurrentDictionary<>
//TryAdd: This method is used to add an item in the ConcurrentDictionary instance. Note that this method throws an exception if the key is already present in the collection.
//TryGetValue: This method is used to retrieve an item from the collection.
//TryRemove: This method is used to remove an item from the collection.
//TryUpdate: This method is used to update a particular key in the ConcurrentDictionary instance with the new value supplied.