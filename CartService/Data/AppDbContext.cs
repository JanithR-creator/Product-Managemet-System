using CartService.Models.Enitity;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(cart =>
            {
                cart.HasKey(cart => cart.CartId);
                cart.Property(cart => cart.UserId).IsRequired();
            });

            modelBuilder.Entity<CartItem>(item =>
            {
                item.HasKey(item => item.CartItemId);
                item.Property(cart => cart.ProductId).IsRequired();
                item.Property(cart => cart.Quantity).IsRequired();
                item.Property(cart => cart.CartId).IsRequired();
                item.Property(cart => cart.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                item.Property(cart => cart.AddedDate).IsRequired().HasDefaultValueSql("GETDATE()");
                item.Property(cart => cart.ProductName).IsRequired().HasMaxLength(100);
                item.Property(cart => cart.ProductDescription).HasMaxLength(500);
                item.Property(cart => cart.Provider).IsRequired().HasMaxLength(100);

                item.HasOne(item => item.Cart)
                .WithMany(cart => cart.Items)
                .HasForeignKey(item => item.CartId);
            });
        }
    }
}
