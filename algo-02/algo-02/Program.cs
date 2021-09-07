
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
using System.Text.RegularExpressions;
using System.Threading;

namespace algo_02
{
    class Program
    {
        static void Main(string[] args)
        {
            // run DB stored proc to nuke?
            Modelinterface modelInterface = new Modelinterface();
            modelInterface.NUKEDATABASE();


            //prompt for input on amount to trade with
            int startupAmount = AlgoStartupAmount();
            int walletNumber = modelInterface.CreateNewWallet(startupAmount);
            

            //prompt for symbols to watch
            //search for symbols - create object refrences
            //confirm Y/N loop

            List<string> symbolhistory = new List<string>();
            List<string> symbols = new List<string>();
            LoadSymbols(out symbolhistory, out symbols);

            //log data to db
            modelInterface.AddSymbolToWatchList(symbols);
            modelInterface.InitSymbolHistory(symbolhistory);

            //prompt to begin algo trading
            bool killCommand = false;
            do
            {
                //log data movement
                //Thread.Sleep(300000); Calling Thread.Sleep with a value of Timeout.Infinite causes a thread to sleep until it is interrupted by another thread that calls the Thread.Interrupt method on the sleeping thread, or until it is terminated by a call to its Thread.Abort method. 
                UpdateSymbols(symbols, out symbolhistory);
                modelInterface.UpdateTickers(symbolhistory);
                //analyze stock history and current position -> decision
                foreach (var symbol in symbols)
                {
                    try
                    {
                        DecisionMaker decision = new DecisionMaker();
                        decision.EvaluateSymbol(symbol);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + e.InnerException);
                    }
                }
                //sleep -10min - may need to review updating tickers for extended times

                //algo trade monitoring and logic - allow for interrupt between system threads 


            } while (!killCommand);
            //exit trading

            //full audit report to CSV? - DB object 

            //display gains or losses (chart? - csv?)

            //prompt for continue with data - start over (without killing DB?) - print and exit

        }

        static int AlgoStartupAmount()
        {
            int startupAmount = 0;
            Console.WriteLine("How much money do I get to play with? ");
            Console.WriteLine("(enter amount and press enter)");
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
                Console.WriteLine($"{e.Message} Please Enter a whole number and press enter.");
                AlgoStartupAmount();

            }
            return startupAmount;

        }

        static void LoadSymbols(out List<string> symbolhistory, out List<string> symbols)
        {
            //create a List<T> where T is a list of Stock Object Models 
            //loop through recieving stock symbols (validate existance) -> creating List<string>
            //push symbol list to logic layer to create the models and interfaceDB
            bool exitBool = false;
            MarketInterface marketInterface = new MarketInterface();
            List<string> historyData = new List<string>();
            List<string> symbolList = new List<string>();

            
            do
            {
                //work- here


                //enter prompt
                Console.WriteLine("Enter the Stock symbol you would like to track and trade. then hit enter");
                try
                {
                    string symbolInput = Console.ReadLine().Trim(' ').ToUpper();
                    string symbolResponse = marketInterface.History_QueryMarket_Symbol_Full(symbolInput);
                    //if query string returns null - throw new
                    if (symbolResponse.Contains("Invalid API call."))
                    {
                        throw new Exception($"The symbol {symbolInput} was not found, hit \"Y\" to try another");
                    }
                    else
                    {
                        Console.WriteLine($"The symbol{symbolInput} was found");
                        Console.WriteLine(symbolResponse);
                        historyData.Add(symbolResponse);
                        symbolList.Add(symbolInput);
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    
                }

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
                        throw new Exception("\r\nplease press the \'Y\' key or the \'N\' key");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    
                }
            } while (!exitBool);

            symbolhistory = historyData;
            symbols = symbolList;

        }

        static void UpdateSymbols(List<string> symbolList ,out List<string> symbolhistory)
        {
            bool exitbool = false;
            MarketInterface marketInterface = new MarketInterface();
            List<string> historyData = new List<string>();
            do
            {
                try
                {
                    foreach (var symbol in symbolList)
                    {
                        string symbolResponse = marketInterface.History_QueryMarket_Symbol(symbol);
                        historyData.Add(symbolResponse);
                    }
                    exitbool = true;
                }
                catch (Exception e)
                {

                    Console.WriteLine("there was an error updating ---> " + e.Message);
                }
                //may need thread sleep
            } while (!exitbool);

            symbolhistory = historyData;
        }
        

    }
}
