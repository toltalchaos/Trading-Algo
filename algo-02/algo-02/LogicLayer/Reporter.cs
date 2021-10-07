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
        
        public Reporter(int startupAmount, int walletNumber)
        {
            _StartupAmount = startupAmount;
            _WalletNumber = walletNumber;
            try
            {
                using (var context = new AlgoDBContext())
                {
                    currentHistory = (from x in context.WALLET_HISTORY select x).OrderByDescending(y => y.transactionNumber).ToList();
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
                using(System.IO.StreamWriter auditReport = new System.IO.StreamWriter(@filePath, true))
                {
                    // transaction#, Direction, Symbol,amount#, amount$, balance

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("big oops in audit trail " + e.Message);
                Console.ReadLine();
            }
        }


    }
}
