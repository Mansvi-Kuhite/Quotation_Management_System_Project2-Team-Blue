using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Models;
using QuotationManagement.API.Services;
using QuotationManagement.API.Data;

namespace QuotationManagement.API.CQRS.Handlers
{
    public class QuoteQueryHandler
    {
        private readonly AppDbContext _context;
        private readonly CacheServices _cacheService;

        public QuoteQueryHandler(AppDbContext context, CacheServices cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // ✅ Get all quotes
        public async Task<List<Quote>> GetAllQuotes()
        {
            return await _context.Quotes.Include(q => q.LineItems).ToListAsync();
        }

        // ✅ Get quote by ID
        public async Task<Quote?> GetQuoteById(int id)
        {
            return await _context.Quotes
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.QuoteId == id);
        }

        // ✅ Analytics with Redis caching
        public async Task<object> GetAnalytics()
        {
            string cacheKey = "analytics_cache";

// Try Redis first
            var cachedAnalytics = await _cacheService.GetCacheAsync<object>(cacheKey);  // no change needed
            if (cachedAnalytics != null)
            {
                Console.WriteLine("✅ Analytics served from Redis cache");
                return cachedAnalytics;
            }

// Compute analytics from DB
            var allQuotes = await _context.Quotes.Include(q => q.LineItems).ToListAsync();
            var totalQuotes = allQuotes.Count;
            var totalValue = allQuotes.Sum(q => q.LineItems.Sum(i => i.UnitPrice * i.Quantity - i.Discount));
            var acceptedCount = allQuotes.Count(q => q.Status == "Accepted");
            var successRate = totalQuotes > 0 ? (acceptedCount * 100) / totalQuotes : 0;
            var avgValue = totalQuotes > 0 ? totalValue / totalQuotes : 0;

            var analytics = new
            {
                totalQuotes,
                totalValue,
                avgValue,
                successRate
            
            };

// Save to Redis as string
            await _cacheService.SetCacheAsync(cacheKey, analytics, TimeSpan.FromMinutes(5));  // ⚡ fixed

            return analytics;
        }
    }
}