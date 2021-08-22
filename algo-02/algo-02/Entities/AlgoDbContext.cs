using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace algo_02.Entities
{
    public partial class AlgoDbContext : DbContext
    {
        public AlgoDbContext()
            : base("name=DbContext")
        {
        }

        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public virtual DbSet<Stock_Item> Stock_Item { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Portfolio>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<Portfolio>()
                .Property(e => e.PurchasePrice)
                .HasPrecision(6, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Open)
                .HasPrecision(6, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.High)
                .HasPrecision(6, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Low)
                .HasPrecision(6, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Close)
                .HasPrecision(6, 4);

            modelBuilder.Entity<Wallet>()
                .Property(e => e.CurrentBalance)
                .HasPrecision(15, 2);
        }
    }
}
