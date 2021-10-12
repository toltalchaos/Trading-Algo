using algo_02.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algo_02.LogicLayer
{
    class Reporter
    {
        int _StartupAmount;
        int _WalletNumber;
        List<WALLET_HISTORY> currentHistory;
        Wallet wallet;
        
        public Reporter(int startupAmount, int walletNumber)
        {
            _StartupAmount = startupAmount;
            _WalletNumber = walletNumber;
            try
            {
                using (var context = new AlgoDBContext())
                {
                    currentHistory = (from x in context.WALLET_HISTORY select x).OrderByDescending(y => y.transactionNumber).ToList();
                    wallet = (from x in context.Wallets where x.WalletNumber == _WalletNumber select x).First();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("big oops in init audit trail " + e.Message);
                Console.ReadLine();
            }
            
        }
        public void CreateAuditTrail(string filePath)
        {
            try
            {
                    //select all rows from wallet history and create an audit report
                using(System.IO.StreamWriter auditReport = new System.IO.StreamWriter(@filePath, false))
                {
                    auditReport.WriteLine($"Starting amount =,{_StartupAmount}, , Ending amount =,{wallet.CurrentBalance}");
                    // transaction#, Direction, Symbol,amount#, amount$, balance
                    auditReport.WriteLine("Transaction Number, Direction, Symbol, Amount of Shares, Amount in Dollars, Current Balance");
                    foreach (var transaction in currentHistory)
                    {
                        auditReport.WriteLine($"{transaction.transactionNumber},{transaction.Direction},{transaction.Symbol},{transaction.Shares},{transaction.Amount},{transaction.Balance}");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("big oops in audit trail ---->" + e.Message);
                Console.ReadLine();
            }
        }


    }
}
