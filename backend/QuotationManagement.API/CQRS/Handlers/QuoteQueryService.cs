using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Data;
using QuotationManagement.API.DTOs;

namespace QuotationManagement.API.CQRS.Handlers
{
    public class QuoteQueryService
    {
        private readonly AppDbContext _context;

        public QuoteQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetAnalytics()
        {
            var allQuotes = await _context.Quotes
            .Include(q => q.LineItems)
            .ToListAsync();

            var totalQuotes = allQuotes.Count;
            var totalValue = allQuotes.Sum(q => q.LineItems.Sum(i => i.UnitPrice * i.Quantity - i.Discount));
            var acceptedCount = allQuotes.Count(q => q.Status == "Accepted");
            var successRate = totalQuotes > 0 ? (acceptedCount * 100) / totalQuotes : 0;
            var avgValue = totalQuotes > 0 ? totalValue / totalQuotes : 0;

    // Optional: status counts
            var statusCount = allQuotes
            .GroupBy(q => q.Status)
            .ToDictionary(g => g.Key, g => g.Count());

            return new
            {
                totalQuotes,
                totalValue,
                avgValue,
                successRate,
                statusCount
            };
        }
    }
}