using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Data;
using QuotationManagement.API.Models;
using QuotationManagement.API.Services;

namespace QuotationManagement.API.CQRS.Handlers
{
    public class QuoteQueryHandler
    {
        private readonly AppDbContext _context;
        private readonly CacheServices _cacheService;
        private readonly ILogger<QuoteQueryHandler> _logger;

        public QuoteQueryHandler(
            AppDbContext context,
            CacheServices cacheService,
            ILogger<QuoteQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Quote>> GetAllQuotes()
        {
            try
            {
                return await _context.Quotes
                    .AsNoTracking()
                    .Include(q => q.LineItems)
                    .ToListAsync();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while loading all quotes.");
                throw new InvalidOperationException("Database error while loading quotes.", ex);
            }
        }

        public async Task<Quote?> GetQuoteById(int id)
        {
            try
            {
                return await _context.Quotes
                    .AsNoTracking()
                    .Include(q => q.LineItems)
                    .FirstOrDefaultAsync(q => q.QuoteId == id);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while loading quote {QuoteId}", id);
                throw new InvalidOperationException("Database error while loading quote details.", ex);
            }
        }

        public async Task<object> GetAnalytics()
        {
            const string cacheKey = "analytics_cache";

            var cachedAnalytics = await _cacheService.GetCacheAsync<object>(cacheKey);
            if (cachedAnalytics != null)
            {
                _logger.LogInformation("Analytics served from cache.");
                return cachedAnalytics;
            }

            var allQuotes = await _context.Quotes
                .AsNoTracking()
                .Include(q => q.LineItems)
                .ToListAsync();

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

            await _cacheService.SetCacheAsync(cacheKey, analytics, TimeSpan.FromMinutes(5));
            return analytics;
        }
    }
}
