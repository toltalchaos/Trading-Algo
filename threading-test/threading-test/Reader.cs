using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threading_test
{
    class Reader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        //create new class per thread, init class to init thread.
        //timeout on WaitOne() returns true after timeout or until current thread recieves input
        //call this class each time a new kill command needs to be checked.
        static Reader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                return input;
            else
                throw new TimeoutException("User did not provide input kill command within the timelimit.");
        }
    }
}
