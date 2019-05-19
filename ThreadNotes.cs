//-------PARALEL PROGRAMLAMA (Thread)--------
Library => using System.Threading;

//1- Thread Types

 //* Foreground vs Background
//-The default value of thread is foreground. But you can make it background by assigning a simple property (IsBackground = true).
//-if Background mode is true, the created thread bind main thread. The created thread ends when main thread end.
//-Yeni oluşturulan thread foreground modda oluşturulur. Ana thread ile bir mağlantısı yoktur. Ana thread sonlansa bile çalışmaya devam eder. Ancak background moda aldığımızda ana threade bağlanır. Ana thread sonlandırıldığında çalışan threadimiz de sonlanır.




//2- Static Properties
Thread thread = new Thread(new ThreadStart(function));
/*
a- Name
	a- thread.Name = "threadName";

b- Priorty
	a- thread.Priority = ThreadPriority.Lowest; .... ThreadPriority.Highest;

c- CurrentThread
	a- Thread.CurrentThread.Name = "main";
	b- Get thread id -> Thread.CurrentThread.ManagedThreadId;

d- IsAlive
	a- thread.IsAlive -> True,False

e- ThreadState
	a- Unstarted
	b- Running
	c- Stopped
	.....

f- IsBackground
	a- thread.IsBackground -> True,False
*/




//3- Static Methods
/*
a- Sleep() //Blocks for a given time period. - thread.Sleep(1000); - 1 second
b- Start() //Start assigned thread object. - thread.Start(if exist parameter, object type);
c- Suspend() // There is no in .net core
d- Resume() // There is no in .net core
e- Join() //Wait for another thread to finish - thread.Join();
f- Abort() // Cancel the worker thread. There is no in .net core
*/



//----SYNCHRONIZATION----
//=> The Lock Mechanism
//***One of the problems that Threads cause is Deadlock

static object locker = new object();
lock(locker)
{ 
	//Threadsafe code 
};

=> Create Basic Thread 
Thread thread = new Thread(functionName);
thread.Start();
or -> new Thread(functionName).Start();
thread.Join(); //Wait(block) until thread ends.

//=> Number of Processor
Environment.ProcessorCount;

//=> Thread Start
//***Zor yöntem!

//Without parameter
Thread thread = new Thread (new ThreadStart (functionName));
thread.Start();
//With parameter (parametre obje biçiminde alinmasi gerekli)
Thread thread = new Thread(new ParameterizedThreadStart(functionName));
thread.Start((object)myObject);

Thread thread = new Thread(functionName); //otomatik algilasin
thread.Start(if exist parameter);


//***Kolay Yontem!
Thread thread = new Thread(delegate() {functionName});
Thread thread = new Thread(() => {functionName});
Thread thread = new Thread(delegate() {functionName(parameter1,parameter2)});
thread.Start();

//4- ThreadPool 
/*
ThreadPool yani thread havuzu threadlerin bir kuyruk şeklinde işlem görmesini sağlar.
Bir thread oluşturulması zaman alan bir işlemdir. aynı işlemi birden fazla yaparken her defasında yeni thread oluşturulması zaman alacaktır.
Bu duruma bir çözüm olarak threadpool çalıştırılan threadleri yok etmeden beklemede tutar. Daha önce yapılan aynı işlemler için yeni thread 
oluşturulmasını engeller. Böylece gözle görülür bir hız artışı yaşanır.

Task yapısı da performans açısından threadpool yapısını arka planda kullanır.

ThreadPool is basically help to manage and reuse the free threads. In other words a threadpool is the collection of background thread.
*/


//örnek kullanım.
WaitCallback threadpoolItem = new WaitCallback(funtionName);
ThreadPool.QueueUserWorkItem(threadpoolItem,(object)parameter);

//çalıştığımız thread threadpool içerisinde bulunuyor mu?
Thread.CurrentThread.IsThreadPoolThread



//----Basic Synchronization----
/*
1- Simple Blocking Methods
	a- Sleep -> Blocks for a given time period.
	b- Join -> Blocks until thread end.

2- Locking Constructs
	a- lock -> Ensures just one thread can access a resource of section of code.
		static object locker = new object();
		lock (locker) 
		{
			//Threadsafe code
		};

	b- Wait Handles
		b1- AutoResetEvent
		b2- ManualResetEvent 
			b2a- Mutex 
			b2b- Semaphore
			b2c- WaitAny, WaitAll, WhenAll and SignalAndWait
*/




//----Extra----
//Abort .net core'da kullanilmiyor. onun yerine CancellationTokenSource() kullaniliyor.
//Thread'ler method olmadığından geri dönüş değerleri yoktur.




//-----InterLocked
int value = 0;
// Add one.
Interlocked.Add(ref value, 1);

// Add one then subtract one.
Interlocked.Increment(ref value);
Interlocked.Decrement(ref value);

//Read
Console.WriteLine(Interlocked.Read(ref value));

// Replace value with 10.
Interlocked.Exchange(ref value, 10);

// CompareExchange: if 10, change to 20.
long result = Interlocked.CompareExchange(ref value, 20, 10);

//**performance
//const int _max = 10000000;
//40.02 ns [Lock] //(value++)
// 6.40 ns [Interlocked.Increment]