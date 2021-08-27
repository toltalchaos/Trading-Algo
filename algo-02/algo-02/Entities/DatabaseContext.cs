using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace algo_02.Entities
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }

        public virtual DbSet<Portfolio> Portfolios { get; set; }
        public virtual DbSet<Stock_Item> Stock_Item { get; set; }
        public virtual DbSet<SYMBOL_HISTORY> SYMBOL_HISTORY { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<WALLET_HISTORY> WALLET_HISTORY { get; set; }
        public virtual DbSet<WatchList> WatchLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Portfolio>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<Portfolio>()
                .Property(e => e.SalePrice)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Open)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.High)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Low)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Stock_Item>()
                .Property(e => e.Close)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SYMBOL_HISTORY>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<SYMBOL_HISTORY>()
                .Property(e => e.Open)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SYMBOL_HISTORY>()
                .Property(e => e.High)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SYMBOL_HISTORY>()
                .Property(e => e.Low)
                .HasPrecision(10, 4);

            modelBuilder.Entity<SYMBOL_HISTORY>()
                .Property(e => e.Close)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Wallet>()
                .Property(e => e.CurrentBalance)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Wallet>()
                .Property(e => e.LastTransactionDirection)
                .IsUnicode(false);

            modelBuilder.Entity<WALLET_HISTORY>()
                .Property(e => e.Balance)
                .HasPrecision(15, 2);

            modelBuilder.Entity<WALLET_HISTORY>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<WALLET_HISTORY>()
                .Property(e => e.Amount)
                .HasPrecision(10, 4);

            modelBuilder.Entity<WALLET_HISTORY>()
                .Property(e => e.Direction)
                .IsUnicode(false);

            modelBuilder.Entity<WatchList>()
                .Property(e => e.symbol)
                .IsUnicode(false);
        }
    }
}
