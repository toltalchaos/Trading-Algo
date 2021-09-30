using algo_02.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using algo_02.EntityModels;

namespace algo_02.LogicLayer
{
    class Modelinterface
    {
        #region init methods
        public void NUKEDATABASE()
        {
            using (var context = new AlgoDBContext())
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

                if (portfolioCollection != null)
                {

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
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("init nuke " + e.InnerException);
                    }
                }




            }
        }
        public int CreateNewWallet(int startupamount)
        {

            using (var context = new AlgoDBContext())
            {
                try
                {
                    Portfolio newPort = new Portfolio();
                    newPort.PortfolioLineNumber = 100;

                    context.Portfolios.Add(newPort);
                    context.SaveChanges();
                    Wallet newWallet = new Wallet();
                    newWallet.WalletNumber = 10;
                    newWallet.CurrentBalance = decimal.Parse(startupamount.ToString());
                    context.Wallets.Add(newWallet);

                    context.SaveChanges();
                    return (from x in context.Wallets select x.WalletNumber).FirstOrDefault();

                }
                catch (Exception e)
                {

                    Console.WriteLine(e.InnerException);
                    return 0;
                }


            }
        }
        public void AddSymbolToWatchList(List<string> symbols)
        {
            //crack open DB context
            using (var context = new AlgoDBContext())
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

                    foreach (var timeStamp in timeSeriesArray)
                    {
                        //create object to submit to DB here
                        SymbolObject newEntry = new SymbolObject();

                        newEntry.Symbol = metaData.First["2. Symbol"].ToString();
                        newEntry.Open = decimal.Parse(timeStamp.First["1. open"].ToString());
                        newEntry.High = decimal.Parse(timeStamp.First["2. high"].ToString());
                        newEntry.Low = decimal.Parse(timeStamp.First["3. low"].ToString());
                        newEntry.Close = decimal.Parse(timeStamp.First["4. close"].ToString());
                        newEntry.Volume = int.Parse(timeStamp.First["5. volume"].ToString());
                        newEntry.DataTime = DateTime.Parse(timeStamp.ToString().Split('"')[1].Trim('"'));

                        // add the first instance to stock item to create the current data for the item
                        if (newEntry.DataTime != DateTime.Parse(timeSeriesArray[0].ToString().Split('"')[1].Trim('"')))
                        {
                            SYMBOL_HISTORY historydatapoint = new SYMBOL_HISTORY();
                            historydatapoint.Symbol = newEntry.Symbol;
                            historydatapoint.Open = newEntry.Open;
                            historydatapoint.High = newEntry.High;
                            historydatapoint.Low = newEntry.Low;
                            historydatapoint.Close = newEntry.Close;
                            historydatapoint.Volume = newEntry.Volume;
                            historydatapoint.DataTime = newEntry.DataTime;
                            //add stock item history datapoint
                            AddStockItemHistoryPointToDB(historydatapoint);
                        }
                        else
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

                }
                catch (Exception e)
                {

                    Console.WriteLine("(initsymbolhistory) there was a problem converting Json data: " + e.Message);
                    Console.ReadKey();
                }


            }

        }
        private void AddStockItemHistoryPointToDB(SYMBOL_HISTORY itemToAdd)
        {
            using(var context = new AlgoDBContext())
            {
                context.SYMBOL_HISTORY.Add(itemToAdd);
                context.SaveChanges();
            }
        }
        private void AddCurrentStockItemtoDB(Stock_Item itemtoadd)
        {
            try
            {
                using (var context = new AlgoDBContext())
                {
                    context.Stock_Item.Add(itemtoadd);
                    context.SaveChanges();

                    string test = (from x in context.Stock_Item where x.Symbol == itemtoadd.Symbol select x.Symbol).FirstOrDefault();
                    Console.WriteLine("selected this symbol object from database, successfull interaction symbol = " + test.Trim());
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("database error ->" + e.InnerException);
            }

        }
        #endregion
        public void UpdateTickers(List<string> symbolData)
        {
            using (var context = new AlgoDBContext())
            {
                //get symbols list
                List<string> symbols = (from x in context.WatchLists select x.symbol).ToList();

                foreach (var symbol in symbols)
                {
                    #region database trigger handles - code left
                    //move current data from the item to the stock item to the item history
                    //create new history entry
                    //SYMBOL_HISTORY updatehistory = new SYMBOL_HISTORY();
                    //updatehistory.Symbol = symbol;
                    //updatehistory.Open = (from x in context.Stock_Item where x.Symbol == symbol select x.Open).FirstOrDefault();
                    //updatehistory.High = (from x in context.Stock_Item where x.Symbol == symbol select x.High).FirstOrDefault();
                    //updatehistory.Low = (from x in context.Stock_Item where x.Symbol == symbol select x.Low).FirstOrDefault();
                    //updatehistory.Close = (from x in context.Stock_Item where x.Symbol == symbol select x.Close).FirstOrDefault();
                    //updatehistory.Volume = (from x in context.Stock_Item where x.Symbol == symbol select x.Volume).FirstOrDefault();
                    //updatehistory.DataTime = (from x in context.Stock_Item where x.Symbol == symbol select x.DataTime).FirstOrDefault();
                    ////fill with data selected from symbolitem table

                    //context.SYMBOL_HISTORY.Add(updatehistory);
                    //change old data
                    #endregion
                    foreach (var dataset in symbolData)
                    {
                        try
                        {


                            JToken symbolObject = JToken.Parse(dataset);
                            //looking for matching symbols
                            if (symbolObject.First.First["2. Symbol"].ToString() == symbol)
                            {
                                //grabbed correct matching symbol
                                JToken firstTimeData = symbolObject.Last.First.First;

                                Stock_Item itemToChange = (from x in context.Stock_Item where x.Symbol == symbol select x).FirstOrDefault();
                                if (itemToChange.DataTime != DateTime.Parse(firstTimeData.ToString().Split('"')[1].Trim('"')).AddMinutes(5))
                                {
                                itemToChange.Open = decimal.Parse(firstTimeData.First["1. open"].ToString());
                                itemToChange.High = decimal.Parse(firstTimeData.First["2. high"].ToString());
                                itemToChange.Low = decimal.Parse(firstTimeData.First["3. low"].ToString());
                                itemToChange.Close = decimal.Parse(firstTimeData.First["4. close"].ToString());
                                itemToChange.Volume = int.Parse(firstTimeData.First["5. volume"].ToString());
                                itemToChange.DataTime = DateTime.Parse(firstTimeData.ToString().Split('"')[1].Trim('"')).AddMinutes(5);
                                context.Entry(itemToChange).State = System.Data.Entity.EntityState.Modified;

                                }

                                //additional additions to timedata here - create list from timedata - select and save non existing times
                                // add times to history data under this symbol
                                List<DateTime> existingtimepoints = (from x in context.SYMBOL_HISTORY where x.Symbol == symbol select x.DataTime).ToList();
                                List<JToken> incomingtimepoints = symbolObject.Last.First.Children().ToList();
                                List<JToken> outgoingTimePoints = new List<JToken>();
                                //create new outgoing to DB list
                                foreach (var incomingTimePoint in incomingtimepoints)
                                {
                                    DateTime incomingTime = DateTime.Parse(incomingTimePoint.ToString().Split('\"')[1]);
                                    if (existingtimepoints.Contains(incomingTime))
                                    {
                                        //Console.WriteLine("this time signature for this symbol already exists");
                                    }
                                    else
                                    {
                                        outgoingTimePoints.Add(incomingTime);
                                    }
                                }
                                //iterate through outgoing times to add to DB
                                foreach (var timePoint in outgoingTimePoints)
                                {
                                    SYMBOL_HISTORY historyDataPoint = new SYMBOL_HISTORY();
                                    historyDataPoint.Symbol = symbol;
                                    historyDataPoint.Open = decimal.Parse(timePoint.First["1. open"].ToString());
                                    historyDataPoint.High = decimal.Parse(timePoint.First["2. high"].ToString());
                                    historyDataPoint.Low = decimal.Parse(timePoint.First["3. low"].ToString());
                                    historyDataPoint.Close = decimal.Parse(timePoint.First["4. close"].ToString());
                                    historyDataPoint.Volume = int.Parse(timePoint.First["5. volume"].ToString());
                                    historyDataPoint.DataTime = DateTime.Parse(timePoint.ToString().Split('"')[1].Trim('"'));
                                    context.SYMBOL_HISTORY.Add(historyDataPoint);
                                    Console.WriteLine("added new historical data to evaluate");
                                }
                            }

                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine("(UpdateTickers)there was a problem " + e.Message + e.InnerException);
                            Console.ReadKey();
                        }
                    }


                }
            }
        }

        public void StockTransaction(string symbol, int walletNumber, int buySellIndex)
        {
            //obtain wallet data
            //obtain symbol existence in portfolio - check valid symbol
            // use buysell index to come up with % of portfolio to spend on stock - error handle
            try
            {
                using (var context = new AlgoDBContext())
                {
                    Wallet wallet = (from x in context.Wallets where x.WalletNumber == walletNumber select x).FirstOrDefault();
                    
                    if (Get_SymbolExistence(symbol))
                    {
                        //find max and min symbol numbers. -> create % purchase logic here
                        Console.WriteLine("buy sell index = " + buySellIndex);
                        if (buySellIndex >= 5)
                        {
                            //buy
                            buySellIndex += -4;
                            decimal percentToBuy = buySellIndex / 10m;
                            Console.WriteLine("BUY " + symbol);

                            //transaction to buy shares up to the total % of the available amount in portfolio
                            // get total amount available - reduce to the % value
                            // get stock item - check for stock item in portfolio (more logic extra hold?)
                            //buy shares -> add to portfolio with purchase logic - check DB triggers (should just need to update portfolio)
                            BuyStock(percentToBuy, symbol, wallet);
                        }
                        else if (buySellIndex < 2)
                        {
                            //sell
                            buySellIndex += 7;
                            Console.WriteLine("SELL " + symbol);
                            SellStock((buySellIndex * 10m), symbol, wallet);
                        }
                        else
                        {
                            //hold
                            
                            Console.WriteLine("hold " + symbol);

                        }
                    }
                    else
                    {
                        throw new Exception("no symbol found, may not be selecting symbols properly?..");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
            
        }

        private void BuyStock(decimal percentOfWallet, string symbol, Wallet wallet)
        {
            using(var context = new AlgoDBContext())
            {
                decimal saleprice = 0;
                Portfolio portfolioItem = (from x in context.Portfolios where x.Symbol == symbol select x).FirstOrDefault();
                Stock_Item symbolData = (from x in context.Stock_Item where x.Symbol == symbol select x).FirstOrDefault();
                //check can afford minimum of 1 share
                if (wallet.CurrentBalance > symbolData.Close)
                {

                if (portfolioItem == null)
                {
                    portfolioItem = new Portfolio();
                        context.Portfolios.Add(portfolioItem);
                        context.SaveChanges();
                        //no portfolio item found - file new                    
                        portfolioItem.Symbol = symbol;
                    // get % of walet balance willing to spend
                    decimal spendingAmount = Math.Round((decimal)wallet.CurrentBalance * percentOfWallet, 2);
                    // number of shares that fit in there(ish) - may eventually throw insufficent funds
                    int numberOfShares = int.Parse(Math.Round(spendingAmount / symbolData.Close, 0).ToString());

                    if(numberOfShares > 0)
                        {
                            Console.WriteLine("debug here " + numberOfShares + " should be a valid int");

                            portfolioItem.SalePrice = (numberOfShares * symbolData.Close) * -1;
                            saleprice = (decimal)portfolioItem.SalePrice;
                            portfolioItem.AmountOwned = numberOfShares;
                            context.Entry(portfolioItem).State = System.Data.Entity.EntityState.Modified;
                        }


                    }
                    else
                    {
                        decimal spendingAmount = Math.Round((decimal)wallet.CurrentBalance * percentOfWallet, 2);
                        int numberOfShares = int.Parse(Math.Round(spendingAmount / symbolData.Close, 0).ToString());

                        Console.WriteLine("debug here " + numberOfShares + " should be a valid int (existing)");
                        if (numberOfShares > 0)
                        {
                            portfolioItem.SalePrice = (numberOfShares * symbolData.Close) * -1;
                            saleprice = (decimal)portfolioItem.SalePrice;
                            portfolioItem.AmountOwned = portfolioItem.AmountOwned + numberOfShares;
                            context.Entry(portfolioItem).State = System.Data.Entity.EntityState.Modified;
                            

                        }


                    }
                    try
                    {

                        context.SaveChanges();


                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("error -  may be insufficent funds " + e.Message + e.InnerException);
                    }
                
                }
            }
            
            
        }
        private void SellStock(decimal percentOfOwned, string symbol, Wallet wallet)
        {
            using (var context = new AlgoDBContext())
            {
                Portfolio portfolioItem = (from x in context.Portfolios where x.Symbol == symbol select x).FirstOrDefault();
                Stock_Item symbolData = (from x in context.Stock_Item where x.Symbol == symbol select x).FirstOrDefault();
                //check can afford minimum of 1 share
                if (wallet.CurrentBalance > symbolData.Close)
                {

                    if (portfolioItem == null)
                    {
                        //do nothing - nothing to sell
                    }
                    else
                    {
                        //number of shares to sell based on index % value
                        int numberOfShares = int.Parse(Math.Round((decimal.Parse(portfolioItem.AmountOwned.ToString())) * (percentOfOwned/100), 0).ToString());

                        Console.WriteLine("debug here" + numberOfShares + "should be a valid int (existing)");

                        portfolioItem.SalePrice = -numberOfShares * symbolData.Close;
                        portfolioItem.AmountOwned = portfolioItem.AmountOwned -numberOfShares;
                        context.Entry(portfolioItem).State = System.Data.Entity.EntityState.Modified;
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("error -  may be insufficent funds" + e.Message + e.InnerException);
                    }

                }
            }
        }

        #region get data
        public bool Get_SymbolExistence(string symbol)
        {
            using(var context = new AlgoDBContext())
            {
                string existingSymbol = (from x in context.WatchLists where x.symbol == symbol select x).FirstOrDefault().ToString();
                if (existingSymbol != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public DateTime Get_SymbolDateTime(string symbol)
        {
            using (var context = new AlgoDBContext())
            {
                Stock_Item thisStockItem = (from x in context.Stock_Item where x.Symbol == symbol select x).ToList().First();
                return (DateTime)thisStockItem.DataTime;
            }
        }
        public SymbolObject Get_SymbolObject_ByDate(DateTime thisDateTime, string symbol)
        {
            using(var context = new AlgoDBContext())
            {

                SymbolObject returnThis = (from x in context.SYMBOL_HISTORY 
                                           where x.DataTime == thisDateTime
                                                && x.Symbol == symbol 
                                           select new SymbolObject{ 
                                                Symbol = x.Symbol,
                                                Open = x.Open,
                                                High = x.High,
                                                Low = x.Low,
                                                Close = x.Close,
                                                Volume = x.Volume,
                                                DataTime = x.DataTime
                                                                        }).FirstOrDefault();
                if (returnThis != null)
                {
                //Console.WriteLine(returnThis.DataTime.ToString());
                }    
                
                return returnThis;
            }
        }
        public List<SYMBOL_HISTORY> Get_SymbolHistory(string symbol)
        {
            using (var context = new AlgoDBContext())
            {
                return (from x in context.SYMBOL_HISTORY where x.Symbol == symbol select x).OrderBy(y => y.DataTime).ToList();
            }
        }
        public Stock_Item Get_StockItem(string symbol)
        {
            using (var context = new AlgoDBContext())
            {
                return (from x in context.Stock_Item where x.Symbol == symbol select x).First();
            }
        }


        #endregion

        #region set data

        #endregion


    }
}
