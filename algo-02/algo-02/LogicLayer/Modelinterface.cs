using algo_02.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace algo_02.LogicLayer
{
    class Modelinterface
    {
        public void NUKEDATABASE()
        {
            using (var context = new DatabaseContext())
            {
                //select table by table in order of dependant to non dependant and nuke all data
                List<SYMBOL_HISTORY> symbolHistoryCollection = (from x in context.SYMBOL_HISTORY
                                                                select x).ToList();
                List<WALLET_HISTORY> walletHistoryCollection = (from x in context.WALLET_HISTORY
                                                                select x).ToList();
                List<Wallet> walletCollection = (from x in context.Wallets
                                                 select x).ToList();
                List<Portfolio> portfolioCollection = (from x in context.Portfolios
                                                       select x).ToList();
                List<Stock_Item> stockCollection = (from x in context.Stock_Item
                                           select x).ToList();
                List<WatchList> watchListCollection = (from x in context.WatchLists
                                                       select x).ToList();
                
                foreach (var item in symbolHistoryCollection)
                {
                    context.SYMBOL_HISTORY.Remove(item);
                }
                foreach (var item in walletHistoryCollection)
                {
                    context.WALLET_HISTORY.Remove(item);
                }
                foreach (var item in walletCollection)
                {
                    context.Wallets.Remove(item);
                }
                foreach (var item in portfolioCollection)
                {
                    context.Portfolios.Remove(item);
                }
                foreach (var item in stockCollection)
                {
                    context.Stock_Item.Remove(item);
                }
                foreach (var item in watchListCollection)
                {
                    context.WatchLists.Remove(item);
                }
                context.SaveChanges();
                
            }
        }
        
        public void AddSymbolToWatchList(List<string> symbols)
        {
            //crack open DB context
            using (var context = new DatabaseContext())
            {
                try
                {
                    foreach (var item in symbols)
                    {
                        WatchList symbolToAdd = new WatchList();
                        symbolToAdd.symbol = item;
                        context.WatchLists.Add(symbolToAdd);
                        Console.WriteLine("\t\n" + item + " added to database");
                    }
                    context.SaveChanges();
                    Console.WriteLine("Watch list saved");
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                }
            }
            
        }
        public void AddSymbolHistory(List<string> symbolHistory)
        {
            //incoming list of string formatted json data for the symbol history
            //deserialize json data to usable data -> enter each set to DB new row per set
            foreach (var item in symbolHistory)
            {
                try
                {
                    //var symbolHistObj = JsonConvert.DeserializeObject<dynamic>(item);
                    //string testvalue = symbolHistObj.ChildrenTokens[0].Value.ChildrenTokens[1].Value.ToString();
                    //Console.WriteLine(testvalue);
                    //Console.ReadKey();
                    //https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm#Index
                    JObject symbolObject = JObject.Parse(item);

                    JArray timeSeries = (JArray)symbolObject["Time Series (5min)"];
                    JArray metaData = (JArray)symbolObject["Meta Data"];

                    Console.ReadKey();
                    


                }
                catch (Exception e)
                {

                    Console.WriteLine("there was a problem converting Json data : " + e.Message);
                    Console.ReadKey();
                }
               

            }
            
        }
    }
}
