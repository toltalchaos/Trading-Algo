using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Reader timeoutReader = new Reader();
            //start threads
        
            do
            {

                try
                {
                    Console.WriteLine("Please enter \"kill\" within the next 5 seconds. \n to terminate program");
                    string input = Reader.ReadLine(5000);
                    if (input.ToLower() == "kill")
                    {
                        killCommand.Set_KillCommand(true);
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Sorry, you waited too long.");
                }


            } while (!killCommand.Get_KillCommand());

            Console.WriteLine("program terminated");
            Console.ReadLine();

        }

      
    }
}
