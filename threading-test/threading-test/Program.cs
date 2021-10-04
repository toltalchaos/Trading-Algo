﻿using System;
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
            //start threads
        
            do
            {
                var killThread = new Thread(() => KillComandThread(ref killCommand));
                killThread.Start();
                //var workThread = new Thread(() => WorkThread(ref killCommand));
                //workThread.Start();
                Console.WriteLine("loop work being done");
                //workThread.Abort();
                Thread.Sleep(5000);
                Console.WriteLine("loop ending about to abort kill comand");
                killThread.Abort();
            } while (!killCommand.Get_KillCommand());

            Console.WriteLine("program terminated");
            Console.ReadLine();

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
            try
            {

                Console.WriteLine("kill comand start");
                string killcommand = Console.ReadLine();
                if (killcommand == "kill")
                {
                    killCommand.Set_KillCommand(true);
                }
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("no kill command entered--- interrupt");

            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("no kill command entered");
               
            }
            

        }
    }
}
