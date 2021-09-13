using algo_02.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algo_02.LogicLayer
{
    class DecisionMaker
    {
        int _BuySellIndex = 0;
        public DecisionMaker(int buysellindexstarter)
        {
            _BuySellIndex = buysellindexstarter;
        }
        public DecisionMaker() { }

        public int GetBuySellIndex() { return _BuySellIndex; }
        //evaluate
        public void EvaluateSymbol(string symbol)
        {
            //search for symb from DB - exists
            Modelinterface modelinterface = new Modelinterface();
            if (modelinterface.Get_SymbolExistence(symbol))
            {
                Volume_Assess(symbol);// 1-3
                MeanReversion_Assess(symbol);//1-3
                Momentum_Assess(symbol);//-3 - 8
                Momentum_Dipping(symbol);//-5 - 3
            }
            else
            {
                throw new Exception("error trying to find symbol.... not sure why check symbol list");
            }
            
        }
        // outcomes -hold -sell -buy

        //purchase/sell - ref wallet number - portfolio associated to wallet
        //log audit (refDB triggers)

        private void Volume_Assess(string symbol)
        {
            Modelinterface modelinterface = new Modelinterface();
            //get historical data 
            List<SYMBOL_HISTORY> symbolHistory = modelinterface.Get_SymbolHistory(symbol);
            Stock_Item currentItem = modelinterface.Get_StockItem(symbol);
            //evaluate volume average long term (all data)
            int longTermVolumeHistory = 0;
            int shortTermVolumeHistory = 0;
            foreach (var historyDatapoint in symbolHistory)
            {
                longTermVolumeHistory = longTermVolumeHistory + historyDatapoint.Volume;

            }
            longTermVolumeHistory = longTermVolumeHistory / symbolHistory.Count();
            for (int i = 0; i < 50; i++)
            {
                shortTermVolumeHistory = shortTermVolumeHistory + symbolHistory[i].Volume;
            }
            shortTermVolumeHistory = shortTermVolumeHistory / 50;

            if (currentItem.Volume >= longTermVolumeHistory)
            {
                if (currentItem.Volume >= shortTermVolumeHistory)
                {
                    _BuySellIndex = _BuySellIndex + 3;
                }
                else
                {
                    _BuySellIndex = _BuySellIndex + 2;
                }
            }
            else
            {
                if (currentItem.Volume >= shortTermVolumeHistory)
                {
                    _BuySellIndex = _BuySellIndex + 1;
                }
                else
                {
                    _BuySellIndex = _BuySellIndex + 2;
                }
            }

        }
        private void MeanReversion_Assess(string symbol)
        {
            Modelinterface modelinterface = new Modelinterface();
            //get historical data 
            List<SYMBOL_HISTORY> symbolHistory = modelinterface.Get_SymbolHistory(symbol);
            Stock_Item currentItem = modelinterface.Get_StockItem(symbol);
            decimal averageHigh = 0;
            decimal averageLow = 0;
            foreach (var historyDataPoint in symbolHistory)
            {
                averageHigh = averageHigh + historyDataPoint.High;
                averageLow = averageLow + historyDataPoint.Low;
            }
            averageHigh = averageHigh / symbolHistory.Count();
            averageLow = averageLow / symbolHistory.Count();
            //combine the highs and lows to assess mean all time average
            decimal allTimeAverage = (averageHigh + averageLow) / 2;
            decimal currentAverge = (currentItem.High + currentItem.Low) / 2;

            if (allTimeAverage > currentAverge)
            {
                _BuySellIndex = _BuySellIndex + 3;
            }
            else
            {
                _BuySellIndex = _BuySellIndex + 1;
            }
        }
        //could improve momentum by adding values for different % higher or lower then comparison to more timeframes close by (instability)
        private void Momentum_Assess(string symbol)
        {
            try
            {
                // evaluate each momentum benchmark looking for general trends and effect the index appropriately 
                Modelinterface modelinterface = new Modelinterface();
                //get historical data 
                List<SYMBOL_HISTORY> symbolHistory = modelinterface.Get_SymbolHistory(symbol);
                Stock_Item currentItem = modelinterface.Get_StockItem(symbol);
                DateTime currentDate = modelinterface.Get_SymbolDateTime(symbol);
                //get % values % = (currentItem.Close / pastItem) X 100
                //      all time
                decimal pastItem = symbolHistory.Last().Close;
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 0;
                }
                //if - over 30 days, add neg
                else
                {
                    _BuySellIndex += -3;
                }
                //      30 days
                decimal pastRatioToCompare = pastItem;
                
                pastItem = (from x in symbolHistory where x.Symbol == currentItem.Symbol && x.DataTime == (currentDate.AddMonths(-1)) select x.Close).First();
                if (((currentItem.Close / pastItem) * 100) > pastRatioToCompare && pastItem != 0)
                {
                    _BuySellIndex += 1;
                }
                //      1 week 
                pastRatioToCompare = pastItem;
                
                pastItem = (from x in symbolHistory where x.Symbol == currentItem.Symbol && x.DataTime == (currentDate.AddDays(-7)) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > pastRatioToCompare)
                {
                    _BuySellIndex += 2;
                }
                //      1 day
                pastRatioToCompare = pastItem;
                
                pastItem = (from x in symbolHistory where x.Symbol == currentItem.Symbol && x.DataTime == (currentDate.AddDays(-1)) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > pastRatioToCompare)
                {
                    _BuySellIndex += 2;
                }
                //      1 hour
                pastRatioToCompare = pastItem;
               
                pastItem = (from x in symbolHistory where x.Symbol == currentItem.Symbol && x.DataTime == (currentDate.AddHours(-1)) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > pastRatioToCompare)
                {
                    _BuySellIndex += 3;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("likely that we are selecting the wrong times --->" + e.Message + e.InnerException);
            }


        }
        private void Momentum_Dipping(string symbol)
        {
            Modelinterface modelinterface = new Modelinterface();
            //get historical data 
            List<SYMBOL_HISTORY> symbolHistory = modelinterface.Get_SymbolHistory(symbol);
            Stock_Item currentItem = modelinterface.Get_StockItem(symbol);
            decimal oneHour = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddHours(-1) select x.Close).FirstOrDefault();
            decimal last15Min = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddMinutes(-15) select x.Close).FirstOrDefault();
            decimal oneHourTrend = ((currentItem.Close / oneHour) * 100);
            decimal fifteenMinuteTrend = ((currentItem.Close / last15Min) * 100);
            if (oneHourTrend > fifteenMinuteTrend)
            {
                //the long term trend is larger then the trend in the last 15 minutes.
                if (oneHourTrend / 2 > fifteenMinuteTrend)
                {
                    //15 minute trend is less then 1/2 the trend over the past hour. (NOSEDIVE)
                    //look for support? 10 minute trend?
                    _BuySellIndex += 3; //buy the dip
                    
                }
            }
            else
            {
                //15 minutes higher momentum then the past hour (spike)
                _BuySellIndex += -3;

                if (oneHourTrend * 2 <= fifteenMinuteTrend)
                {
                    _BuySellIndex += -2; //huge spike!! dump shares 
                }
            }

        }
    }
}
