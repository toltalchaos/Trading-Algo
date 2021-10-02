using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threading_test
{
    class Program
    {
        static void Main(string[] args)
        {
            //goal is to test doing work during a multithreaded process using a seperate thread to manipulate the state which things occur
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/ee0702ee-444e-4b05-86af-a809ee5fbc96/thread-is-running-or-terminated-it-cannot-restart?forum=csharpgeneral
            //https://docs.microsoft.com/en-us/dotnet/standard/threading/pausing-and-resuming-threads
            Console.WriteLine("init");
            KillObject killCommand = new KillObject();

            //start threads
            do
            {
                var workThread = new Thread(() => WorkThread(ref killCommand));
                var killThread = new Thread(() => KillComandThread(ref killCommand));
                workThread.Start();
                Console.WriteLine("loop work being done");
                workThread.Abort();
                killThread.Start();
                Thread.Sleep(2000);
                killThread.Abort();
            } while (!killCommand.Get_KillCommand());

            

        }

        private static void WorkThread(ref KillObject killCommand)
        {
            try
            {
                Console.WriteLine("work being done");
                
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("work thread stopped");
               
            }  
                

        }
        private static void KillComandThread(ref KillObject killCommand)
        {
            Console.WriteLine("kill comand start");
            Console.ReadLine();

        }
    }
}
