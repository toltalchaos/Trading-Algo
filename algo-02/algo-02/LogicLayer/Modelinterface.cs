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
        public void InitSymbolHistory(List<string> symbolHistory)
        {
            //incoming list of string formatted json data for the symbol history
            //deserialize json data to usable data -> enter each set to DB new row per set
            foreach (var item in symbolHistory)
            {
                try
                {
                    //var symbolHistObj = JsonConvert.DeserializeObject<dynamic>(item);
                   
                    //https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm#Index
                    JToken symbolObject = JToken.Parse(item);
                    JToken metaData = symbolObject.First;
                    JToken timeSeries = JToken.Parse(symbolObject["Time Series (5min)"].ToString());

                    List<JToken> timeSeriesArray = timeSeries.Children().ToList();
                    string test = timeSeriesArray[0].ToString();
                    Console.WriteLine(test);

                    foreach (var timeStamp in timeSeriesArray)
                    {
                        //create object to submit to DB here
                        SYMBOL_HISTORY newEntry = new SYMBOL_HISTORY();
                        
                        Console.WriteLine();
                        newEntry.Symbol = metaData.First["2. Symbol"].ToString();
                        newEntry.Open = decimal.Parse(timeStamp.First["1. open"].ToString());
                        newEntry.High = decimal.Parse(timeStamp.First["2. high"].ToString());
                        newEntry.Low = decimal.Parse(timeStamp.First["3. low"].ToString());
                        newEntry.Close = decimal.Parse(timeStamp.First["4. close"].ToString());
                        newEntry.Volume = int.Parse(timeStamp.First["5. volume"].ToString());
                        newEntry.DataTime = DateTime.Parse(timeStamp.ToString().Split('"')[1].Trim('"'));

                        //add stock item history datapoint
                        AddStockItemHistoryPointToDB(newEntry);

                        // add the first instance to stock item to create the current data for the item
                        if (timeStamp == timeSeriesArray[0])
                        {
                            Stock_Item currentdata = new Stock_Item();
                            currentdata.Symbol = newEntry.Symbol;
                            currentdata.Open = newEntry.Open;
                            currentdata.High = newEntry.High;
                            currentdata.Low = newEntry.Low;
                            currentdata.Close = newEntry.Close;
                            currentdata.Volume = newEntry.Volume;
                            currentdata.DataTime = newEntry.DataTime;
                            AddCurrentStockItemtoDB(currentdata);
                            
                        }

                    }

                    Console.ReadKey();
                }
                catch (Exception e)
                {

                    Console.WriteLine("there was a problem converting Json data: " + e.Message);
                    Console.ReadKey();
                }
               

            }
            
        }

        private string RemoveJsonStringBraces(JToken incomingToken)
        {
            string cleanupString = incomingToken.ToString().TrimStart('{').TrimEnd('}');
            return cleanupString;
        }
        private void AddStockItemHistoryPointToDB(SYMBOL_HISTORY itemToAdd)
        {
            using(var context = new DatabaseContext())
            {
                context.SYMBOL_HISTORY.Add(itemToAdd);
                context.SaveChanges();
            }
        }
        private void AddCurrentStockItemtoDB(Stock_Item itemtoadd)
        {
            using (var context = new DatabaseContext())
            {
                context.Stock_Item.Add(itemtoadd);
                context.SaveChanges();

                string test = (from x in context.Stock_Item where x.Symbol == itemtoadd.Symbol select x).FirstOrDefault().ToString();
                Console.WriteLine("selected this symbol object from database, successfull interaction" + test);
            }
        }
    }
}
