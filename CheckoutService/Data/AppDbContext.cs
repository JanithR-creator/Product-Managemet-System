using CheckoutService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckoutService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<CheckoutItem> CheckoutItems { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Checkout>(checkOut =>
            {
                checkOut.HasKey(c => c.CheckoutId);
                checkOut.Property(c => c.CheckoutId).ValueGeneratedOnAdd();
                checkOut.Property(c => c.UserId).IsRequired();
                checkOut.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
                checkOut.Property(c => c.Status).IsRequired().HasMaxLength(10);
            });

            modelBuilder.Entity<CheckoutItem>(item =>
            {
                item.HasKey(ci => ci.CheckoutItemId);
                item.Property(ci => ci.ProductId).IsRequired();
                item.Property(ci => ci.ProductName).IsRequired().HasMaxLength(100);
                item.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                item.Property(ci => ci.Quantity).IsRequired();
                item.Property(ci => ci.CheckoutId).IsRequired();

                item.HasOne(ci => ci.Checkout)
                    .WithMany(c => c.Items)
                    .HasForeignKey(ci => ci.CheckoutId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<PaymentRecord>(payment =>
            {
                payment.HasKey(p => p.PaymentRecordId);
                payment.Property(p => p.CheckoutId).IsRequired();
                payment.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
                payment.Property(p => p.Status).IsRequired().HasMaxLength(20);
                payment.Property(p => p.Message).IsRequired().HasMaxLength(20);
                payment.Property(p => p.PaidAt).IsRequired().HasDefaultValueSql("GETDATE()");

                payment.HasOne(p => p.Checkout)
                       .WithOne(c => c.Payment)
                       .HasForeignKey<PaymentRecord>(p => p.CheckoutId)
                       .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
