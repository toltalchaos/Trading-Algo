namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SYMBOL_HISTORY
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string Symbol { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public int? Volume { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime DataTime { get; set; }
    }
}
