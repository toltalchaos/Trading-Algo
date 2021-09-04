namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Stock_Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stock_Item()
        {
            Portfolios = new HashSet<Portfolio>();
            WALLET_HISTORY = new HashSet<WALLET_HISTORY>();
        }

        [Key]
        [StringLength(5)]
        public string Symbol { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public int? Volume { get; set; }

        public DateTime DataTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Portfolio> Portfolios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WALLET_HISTORY> WALLET_HISTORY { get; set; }
    }
}
