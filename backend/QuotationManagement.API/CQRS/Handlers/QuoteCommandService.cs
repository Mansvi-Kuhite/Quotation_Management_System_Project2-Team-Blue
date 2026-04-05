using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Data;
using QuotationManagement.API.Helpers;
using QuotationManagement.API.Models;
using QuotationManagement.API.Services;

namespace QuotationManagement.API.CQRS.Handlers
{
    public class QuoteCommandService
    {
        private readonly AppDbContext _context;
        private readonly CacheServices? _cacheService;

        public QuoteCommandService(AppDbContext context, CacheServices? cacheService = null)
        {
            _context = context;
            _cacheService = cacheService;
        }

        private async Task InvalidateAnalyticsCache()
        {
            if (_cacheService == null) return;
            await _cacheService.RemoveCacheAsync("analytics_cache");
        }

        // 🔵 CREATE QUOTE
        public async Task<Quote> CreateQuote(Quote quote, string username)
        {
            var quoteCount = await _context.Quotes.CountAsync();

            quote.QuoteNumber = string.IsNullOrWhiteSpace(quote.QuoteNumber)
                ? QuoteNumberGenerator.Generate(quoteCount + 1)
                : quote.QuoteNumber.Trim();
            quote.Status = "Draft";
            quote.CreatedBy = username;
            quote.CreatedDate = DateTime.UtcNow;
            quote.QuoteDate = quote.QuoteDate == default ? DateTime.UtcNow : quote.QuoteDate;
            quote.ExpiryDate = quote.ExpiryDate == default ? quote.QuoteDate.AddDays(30) : quote.ExpiryDate;
            quote.SubTotal = 0;
            quote.TaxAmount = 0;
            quote.GrandTotal = 0;
            quote.RevisionNumber = quote.RevisionNumber <= 0 ? 1 : quote.RevisionNumber;

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();
            await InvalidateAnalyticsCache();

            return quote;
        }

        // 🟡 ADD LINE ITEM
        public async Task AddLineItem(int id, QuoteLineItem item)
        {
            var quote = await _context.Quotes
                .Include(q => q.LineItems)
                .FirstOrDefaultAsync(q => q.QuoteId == id);

            if (quote == null)
                throw new Exception("Quote not found");

            // 🔴 Lock after Accepted
            if (quote.Status == "Accepted")
                throw new Exception("Cannot modify accepted quote");

            item.LineTotal = (item.Quantity * item.UnitPrice) - item.Discount;
            quote.LineItems.Add(item);
            CalculationHelper.CalculateTotals(quote);

            await _context.SaveChangesAsync();
            await InvalidateAnalyticsCache();
        }

        // 🔴 DELETE QUOTE
        public async Task DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                throw new Exception("Quote not found");

            // 🔴 Lock after Accepted
            if (quote.Status == "Accepted")
                throw new Exception("Cannot delete accepted quote");

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();
            await InvalidateAnalyticsCache();
        }

        // 🟢 CHANGE STATUS (STRICT WORKFLOW)
        public async Task<string> ChangeStatus(int id, string status)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                throw new Exception("Quote not found");

            // 🔴 Lock after Accepted
            if (quote.Status == "Accepted")
                throw new Exception("Cannot modify accepted quote");

            // 🔴 STRICT TRANSITIONS ONLY
            if (quote.Status == "Draft" && status == "Sent")
            {
                quote.Status = "Sent";
            }
            else if (quote.Status == "Sent" && status == "Accepted")
            {
                quote.Status = "Accepted";
            }
            else
            {
                throw new Exception($"Invalid transition from {quote.Status} to {status}");
            }

            await _context.SaveChangesAsync();
            await InvalidateAnalyticsCache();
            return quote.Status;
        }
    }
}
