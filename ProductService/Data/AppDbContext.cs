using Microsoft.EntityFrameworkCore;
using ProductService.Model.Entity;

namespace ProductService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<BookDetails> BookDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(productE =>
            {
                productE.HasKey(productE => productE.ProductId);
                productE.Property(productE=>productE.Provider).IsRequired();
                productE.Property(productE=>productE.Name).IsRequired();
                productE.Property(productE=>productE.Description).IsRequired();
                productE.Property(productE=>productE.Price).IsRequired();
                productE.Property(productE=>productE.Quantity).IsRequired();
                productE.Property(productE=>productE.PruductType).IsRequired();

                productE.HasOne(productE => productE.BookDetails)
                .WithOne(bookDE => bookDE.Product)
                .HasForeignKey<BookDetails>(bookDE => bookDE.ProductId);
            });

            modelBuilder.Entity<BookDetails>(bookDE =>
            {
                bookDE.HasKey(bookDE=>bookDE.detailId);
                bookDE.Property(bookDE=>bookDE.Author).IsRequired();
                bookDE.Property(bookDE=>bookDE.Publisher).IsRequired();
                bookDE.Property(bookDE=>bookDE.Category).IsRequired();
            });
        }
    }
}
