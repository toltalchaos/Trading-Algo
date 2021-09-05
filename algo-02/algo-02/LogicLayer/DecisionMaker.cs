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
        //evaluate
        public void EvaluateSymbol(string symbol)
        {
            //search for symb from DB - exists
            Modelinterface modelinterface = new Modelinterface();
            if (modelinterface.Get_SymbolExistence(symbol))
            {
                Volume_Assess(symbol);

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


    }
}
