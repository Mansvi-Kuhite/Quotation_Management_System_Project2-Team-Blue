using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Data;
using QuotationManagement.API.Models;

namespace QuotationManagement.API.CQRS.Handlers
{
    public class QuoteCommandService
    {
        private readonly AppDbContext _context;

        public QuoteCommandService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE
        public async Task<Quote> CreateQuote(Quote quote)
        {
            quote.CreatedBy = "Admin";
            quote.CreatedDate = DateTime.UtcNow;
            quote.Status = "Draft";

            quote.RevisionNumber = 1;

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return quote;
        }

        // ✅ DELETE
        public async Task<bool> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return false;

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return true;
        }

        // ✅ ADD LINE ITEM
        public async Task<QuoteLineItem> AddLineItem(int quoteId, QuoteLineItem item)
        {
            item.QuoteId = quoteId;

            // ✅ calculate line total
            item.LineTotal = (item.Quantity * item.UnitPrice) - item.Discount;

            _context.LineItems.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        // ✅ CHANGE STATUS
        public async Task<bool> ChangeStatus(int id, string status)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return false;

            quote.Status = status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateStatus(int id, string status)
        {
            var quote = await _context.Quotes.FindAsync(id);
 
            if (quote == null)
            throw new Exception("Quote not found");

            quote.Status = status;

            await _context.SaveChangesAsync();
        }
    }
}