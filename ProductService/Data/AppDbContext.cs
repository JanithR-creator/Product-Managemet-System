using Microsoft.EntityFrameworkCore;
using ProductService.Model.Entity;

namespace ProductService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p=>p.ProductId);
                entity.Property(p=>p.Name).IsRequired();
                entity.Property(p=>p.Description).IsRequired();
                entity.Property(p=>p.Price).IsRequired();
                entity.Property(p=>p.Quantity).IsRequired();

                entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p=>p.CategoryId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(p=>p.CategoryId);
                entity.Property(p=>p.Name).IsRequired();
            });
        }
    }
}
