using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Models;

namespace QuotationManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteLineItem> LineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(x => x.SubTotal).HasPrecision(18, 2);
                entity.Property(x => x.TaxAmount).HasPrecision(18, 2);
                entity.Property(x => x.DiscountAmount).HasPrecision(18, 2);
                entity.Property(x => x.GrandTotal).HasPrecision(18, 2);
            });

            modelBuilder.Entity<QuoteLineItem>(entity =>
            {
                entity.Property(x => x.UnitPrice).HasPrecision(18, 2);
                entity.Property(x => x.Discount).HasPrecision(18, 2);
                entity.Property(x => x.LineTotal).HasPrecision(18, 2);
            });
        }
    }
}
