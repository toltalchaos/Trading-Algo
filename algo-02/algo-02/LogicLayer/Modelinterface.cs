using algo_02.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algo_02.LogicLayer
{
    class Modelinterface
    {
        public void NUKEDATABASE()
        {
            using (var dbcontext = new AlgoDataBase())
            {
                //select table by table in order of dependant to non dependant and nuke all data
                SYMBOL_HISTORY symbHist = null;
                WALLET_HISTORY walletHist = null;
                Wallet wallet = null;
                Portfolio portfolio = null;
                Stock_Item stockItem = null;
                Stock_Item collection = (from x in dbcontext.Stock_Item
                                           select x.Symbol).to;
                Console.WriteLine(collection);
                
            }
        }
        private void NUKEDBHELPER()
        {
            using(var dbcontext = new AlgoDataBase())
            {
                List<string> collection = (from x in dbcontext.Stock_Item
                                           select x.Symbol).ToList();
                foreach (var item in collection)
                {

                }

            }
        }
        public void AddSymbolToWatchList(string symbol)
        {

        }
    }
}
