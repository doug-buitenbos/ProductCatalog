using Microsoft.EntityFrameworkCore;

namespace ProductCatalogApi.Models
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p =>
            {
                p.Property(e => e.Id);
                p.HasKey(e => e.Id);
                p.Property(e => e.Name);
                p.Property(e => e.Sku);
                p.HasMany(e => e.PriceHistory).WithOne().IsRequired();
            });

            modelBuilder.Entity<PriceUpdate>(u =>
            {
                u.Property(e => e.Id);
                u.HasKey(e => e.Id);
                u.Property(e => e.UpdatedDateTime);
                u.Property(e => e.Amount);
            });
        }
    }
}
