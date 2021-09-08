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
                Volume_Assess(symbol);
                MeanReversion_Assess(symbol);
                Momentum_Assess(symbol);
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
        private void Momentum_Assess(string symbol)
        {
            try
            {
                // evaluate each momentum benchmark looking for general trends and effect the index appropriately 
                Modelinterface modelinterface = new Modelinterface();
                //get historical data 
                List<SYMBOL_HISTORY> symbolHistory = modelinterface.Get_SymbolHistory(symbol);
                Stock_Item currentItem = modelinterface.Get_StockItem(symbol);
                //get % values % = (currentItem.Close / pastItem) X 100
                //      all time
                decimal pastItem = symbolHistory.Last().Close;
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 1;
                }
                //if - over 30 days, add neg
                else
                {
                    _BuySellIndex += -2;
                }
                //      30 days
                pastItem = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddMonths(-1) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 2;
                }
                //      1 week 
                pastItem = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddDays(-7) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 3;
                }
                //      1 day
                pastItem = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddDays(-1) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 3;
                }
                //      1 hour
                pastItem = (from x in symbolHistory where x.DataTime == currentItem.DataTime.AddHours(-1) select x.Close).FirstOrDefault();
                if (((currentItem.Close / pastItem) * 100) > 0)
                {
                    _BuySellIndex += 4;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("likely that we are selecting the wrong times --->" + e.Message);
            }


        }
        private void Momentum_GapGo
    }
}
