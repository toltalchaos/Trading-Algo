
// purpose is to run a conesole based stock trading algorythm connected to locally hosted DB 
// using alpaca API to interface with market data
//using console app .net framework

//**** CURRENTLY PAPER TRADING ONLY****



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using algo_02.LogicLayer;
namespace algo_02
{
    class Program
    {
        static void Main(string[] args)
        {
            // run DB stored proc to nuke?
            Modelinterface models = new Modelinterface();


            //prompt for input on amount to trade with
            int startupAmount = AlgoStartupAmount();

            //prompt for symbols to watch
            //search for symbols - create object refrences
            //confirm Y/N loop
            LoadSymbols();

            //log data to db

            //create stock item objects for each symbol

            //prompt to begin algo trading

                //log data movement
                //analyze stock history and current position -> decision
                //sleep -10min

            //algo trade monitoring and logic - allow for interrupt between system threads 

            //exit trading

            //full audit report to CSV? - DB object 

            //display gains or losses (chart?)

            //prompt for continue with data - start over - print and exit

        }

        static int AlgoStartupAmount()
        {
            int startupAmount = 0;
            Console.WriteLine("How much money do I get to play with? ");
            Console.WriteLine("(enter amount and press enter");
            try
            {
                startupAmount = int.Parse(Console.ReadLine());
                if (startupAmount <= 0)
                {
                    throw new Exception("number too low");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message} Please Enter a whole number and press enter)");
                AlgoStartupAmount();

            }
            Console.ReadLine();
            return startupAmount;

        }

        static void LoadSymbols()
        {
            //create a List<T> where T is a list of Stock Object Models 
            //loop through recieving stock symbols (validate existance) -> creating List<string>
            //push symbol list to logic layer to create the models and interfaceDB
            bool exitBool = false;
            do
            {
                //work- here

                Console.WriteLine("are you finished adding stocks to watch? Y/N");
                try
                {
                    ConsoleKeyInfo answer = Console.ReadKey();
                    if (answer.KeyChar == 'y')
                    {
                        exitBool = true;
                    }
                    else
                    {
                        throw new Exception("please press the \'Y\' key or the \'N\' key");
                    }
                }
                catch (Exception e)
                {

                    throw;
                }
            } while (!exitBool);
        }

    }
}
