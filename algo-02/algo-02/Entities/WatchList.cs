namespace algo_02.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WatchList")]
    public partial class WatchList
    {
        [Key]
        [StringLength(5)]
        public string symbol { get; set; }
    }
}
