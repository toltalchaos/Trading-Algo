namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Portfolio")]
    public partial class Portfolio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Portfolio()
        {
            Wallets = new HashSet<Wallet>();
            WALLET_HISTORY = new HashSet<WALLET_HISTORY>();
        }

        [Key]
        public int PortfolioLineNumber { get; set; }

        [StringLength(5)]
        public string Symbol { get; set; }

        public decimal? SalePrice { get; set; }

        public int? AmountOwned { get; set; }

        public virtual Stock_Item Stock_Item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Wallet> Wallets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WALLET_HISTORY> WALLET_HISTORY { get; set; }
    }
}
