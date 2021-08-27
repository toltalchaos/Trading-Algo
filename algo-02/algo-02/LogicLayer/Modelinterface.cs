﻿using algo_02.Entities;
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
        
        public void AddSymbolToWatchList(string symbol)
        {
            //crack open DB context
            using (var context = new DatabaseContext())
            {
                try
                {

                    Console.WriteLine("\t\n" + symbol + " added to database");
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                }
            }
            
        }
    }
}
