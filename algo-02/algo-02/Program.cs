
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
namespace algo_02
{
    class Program
    {
        static void Main(string[] args)
        {
            MarketInterface API = new MarketInterface();

            string testing_ = API.TESTGET().Result;
            Console.WriteLine(testing_);

            Console.WriteLine("press a key to exit");
            Console.ReadLine();

        }
    }
}
