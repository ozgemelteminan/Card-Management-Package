using Microsoft.EntityFrameworkCore;
using CardManagement.Shared.Models; 

namespace CardManagement.Data 
{
    public class AppDbContext : DbContext
    {
        // Constructor: pass DbContextOptions to base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet properties represent tables in the database
        public DbSet<Merchant> Merchants => Set<Merchant>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cardholder> Cardholders => Set<Cardholder>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<TransactionProductDetail> TransactionProductDetails => Set<TransactionProductDetail>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        // Configure relationships, constraints, and precision for entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Transaction → Card relationship
            // One Transaction has one Card, but deleting a Card does not delete Transactions (Restrict)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Card)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionProductDetail → Transaction relationship
            // Each TransactionProductDetail belongs to a Transaction
            // Deleting a Transaction does not delete related TransactionProductDetails (Restrict)
            modelBuilder.Entity<TransactionProductDetail>()
                .HasOne(tpd => tpd.Transaction)
                .WithMany(t => t.ProductDetails)
                .HasForeignKey(tpd => tpd.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionProductDetail → Product relationship
            // Each detail references a Product, deletion restricted
            modelBuilder.Entity<TransactionProductDetail>()
                .HasOne(tpd => tpd.Product)
                .WithMany()
                .HasForeignKey(tpd => tpd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure precision for decimal fields (currency-related)
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Card>().Property(c => c.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<CartItem>().Property(ci => ci.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<CartItem>().Property(ci => ci.TotalPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>().Property(t => t.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionProductDetail>().Property(tpd => tpd.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionProductDetail>().Property(tpd => tpd.TotalPrice).HasPrecision(18, 2);
        }
    }
}
