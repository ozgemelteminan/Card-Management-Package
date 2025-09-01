using Microsoft.EntityFrameworkCore;
using MerchantApp.API.Models;

namespace MerchantApp.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet<T> represents a table in the database
    public DbSet<Merchant> Merchants => Set<Merchant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cardholder> Cardholders => Set<Cardholder>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionProductDetail> TransactionProductDetails => Set<TransactionProductDetail>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Transaction -> Card (optional relationship)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Card)                  // A Transaction has one Card
            .WithMany(c => c.Transactions)        // A Card can have many Transactions
            .HasForeignKey(t => t.CardId)         // Foreign key: CardId
            .OnDelete(DeleteBehavior.Restrict);   // Prevent cascade delete

        // TransactionProductDetail -> Transaction
        modelBuilder.Entity<TransactionProductDetail>()
            .HasOne(tpd => tpd.Transaction)       // Each detail belongs to a Transaction
            .WithMany(t => t.ProductDetails)      // A Transaction has many details
            .HasForeignKey(tpd => tpd.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        // TransactionProductDetail -> Product
        modelBuilder.Entity<TransactionProductDetail>()
            .HasOne(tpd => tpd.Product)           // Each detail references a Product
            .WithMany()                           // Product does not track its details
            .HasForeignKey(tpd => tpd.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Set decimal precision for monetary values
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Card>()
            .Property(c => c.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TransactionProductDetail>()
            .Property(tpd => tpd.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TransactionProductDetail>()
            .Property(tpd => tpd.TotalPrice)
            .HasPrecision(18, 2);
    }
}
