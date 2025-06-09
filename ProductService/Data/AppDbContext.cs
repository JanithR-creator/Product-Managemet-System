using Microsoft.EntityFrameworkCore;
using ProductService.Model.Entity;

namespace ProductService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(productE =>
            {
                productE.HasKey(productE => productE.ProductId);
                productE.Property(productE=>productE.ExternalDbId).IsRequired();
                productE.Property(productE=>productE.Provider).IsRequired();
                productE.Property(productE=>productE.Name).IsRequired();
                productE.Property(productE=>productE.Description).IsRequired();
                productE.Property(productE=>productE.Price).IsRequired();
                productE.Property(productE=>productE.Quantity).IsRequired();
                productE.Property(productE=>productE.PruductType).IsRequired();
                productE.Property(productE=>productE.ImageUrl).IsRequired(false);
            });

            {
                modelBuilder.Entity<Product>()
                    .HasOne(p => p.ProductDetails)
                    .WithOne(pd => pd.Product)
                    .HasForeignKey<ProductDetails>(pd => pd.ProductId);
            }

    }
}
}
