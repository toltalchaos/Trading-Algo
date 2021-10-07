namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WALLET_HISTORY
    {
        [Key]
        public int transactionNumber { get; set; }

        public int PortfolioLineNumber { get; set; }

        public int Shares { get; set; }

        public decimal? Balance { get; set; }

        [StringLength(5)]
        public string Symbol { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(4)]
        public string Direction { get; set; }

        public virtual Portfolio Portfolio { get; set; }

        public virtual Stock_Item Stock_Item { get; set; }
    }
}
