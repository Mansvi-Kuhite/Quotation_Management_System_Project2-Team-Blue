using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Models;

namespace QuotationManagement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteLineItem> LineItems { get; set; }
    }
}