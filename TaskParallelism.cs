//---------Tasks
/*
Tüm Parallel sınıfı ve PLINQ bu Task objelerini kullanarak çalışmaktadırlar.

Task-Bir işin yapılmasını sağlar
Task<TResult>-Geri dönüş değeri olan bir işin yapılmasını sağlar
TaskFactory-Görev başlatmak için kullanılır
TaskFactory<TResult>-Geri dönüş değeri olan bir görev oluşturmak için kullanılır
*/

//Async programlama ile daha sıklıkla kullanılan Task yapısı thread yapısına göre üst seviyede. 
//Task yapısını kullanarak daha gelişmiş işlemler yapabiliriz. 
//Thread pooling yapısını otomatik olarak kullanıp birbiri ardına eklenebilecek olan işlemleri daha iyi organize etmektedir.


//Aşağıdaki yapıda tasks adında türü task olan bir dizi oluşturduk. 
//Bu dizi 3 task değeri alacaktır. Task tanımlamalarımızı yaptıktan sonra Task.WaitAll(tasks) 
//metodu ile tüm taskların çalışıp biteceğini garanti ettik. Aynı thread yapısında bulunan join() metodu gibi

var tasks = new Task[3];

task[0] = Task.Run(() => {Thread.Sleep(1000); Console.WriteLine("1");return 1;});
task[2] = Task.Run(() => {Thread.Sleep(1000); Console.WriteLine("2");return 2;});
task[3] = Task.Run(() => {Thread.Sleep(1000); Console.WriteLine("3");return 3;});

Task.WaitAll(tasks);

//Void biçim task örnek-----------------------
		void EkranaYazdir(string taskAdi)
        {
            for (int k = 0; k < 100; k++) {
                Console.WriteLine(taskAdi + " - " + k);
                Thread.Sleep(50);
            }
        }

        void Main(string[] args)
        {
        	Stopwatch sw = Stopwatch.StartNew();

            Task islem1 = Task.Factory.StartNew(() => EkranaYazdir("Islem1"));

            Task islem2 = Task.Factory.StartNew(() => EkranaYazdir("Islem2"));

            Task.WaitAll(islem1,islem2);

            Console.WriteLine("{0} saniye sürdü", sw.ElapsedMilliseconds / 1000.0);

            Console.Read();
        }


//Geri dönüş tipli task örnek---------------------------
 		private int UcakBiletiAl()
        {
            Console.WriteLine("Ucak bileti alınıyor...");
            Thread.Sleep(1000);
            Console.WriteLine("Ucak bileti alındı...");

            return 10;
        }

        void Main(string[] args)
        {
            Task <int> birinciIslem = Task <int>.Factory.StartNew(UcakBiletiAl);

            Console.WriteLine(birinciIslem.Result);
        }




//-------Paralel vs Asynchronous
/*
Parallel programming is a technique where we use multiple threads to execute a task faster. 
This means that on modern multi-core architectures we can utilize more of the resources available to perform a task.

A great example of this is sorting a list using quicksort.

Normally with parallel programming performance is important and all the threads are working to a common goal.

Asynchronous programming is subtly different. This normally involves longer running tasks 
and tasks which are perhaps waiting on some kind of external stimuli. A good example of this is to perform a large calculation 
in a background thread so that the UI remains responsive. With asynchronous code we are normally talking about code 
which executes at a different rate to our main application.
*/


//-------Blocking Collection

var taskExample1 = Task.Run(()=>{Console.WriteLine("Ex1 Complete.");});
var taskExample2 = Task.Run(()=>{Console.WriteLine("Ex2 Complete.");});

Task.WaitAll(taskExample1,taskExample2);



//-------Flowing excution context with AsyncLocal

void Main()
{
    AsyncLocal<string> user = new AsyncLocal<string>();
    user.Value = "initial user";
    
    // this does not affect other tasks - values are local relative to the branches of execution flow
    Task.Run(() => user.Value = "user from another task"); 
    
    var task1 = Task.Run(() =>
    {
        Console.WriteLine(user.Value); // outputs "initial user"
        Task.Run(() =>
        {
            // outputs "initial user" - value has flown from main method to this task without being changed
            Console.WriteLine(user.Value);
        }).Wait();

        user.Value = "user from task1";

        Task.Run(() =>
        {
            // outputs "user from task1" - value has flown from main method to task1
            // than value was changed and flown to this task.
            Console.WriteLine(user.Value);
        }).Wait();
    });
    
    task1.Wait();
    
    // ouputs "initial user" - changes do not propagate back upstream the execution flow    
    Console.WriteLine(user.Value); 
}


Thread.Sleep (TimeSpan.FromHours (24));