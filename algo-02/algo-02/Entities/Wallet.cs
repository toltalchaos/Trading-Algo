namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Wallet")]
    public partial class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WalletNumber { get; set; }

        public int? PortfolioNumber { get; set; }

        public decimal? CurrentBalance { get; set; }

        public virtual Portfolio Portfolio { get; set; }
    }
}
