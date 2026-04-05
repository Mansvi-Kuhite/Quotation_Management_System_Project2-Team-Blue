using System;

namespace QuotationManagement.API.DTOs
{
    public class CreateQuoteDto
    {
        public int CustomerId { get; set; }

        public DateTime QuoteDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        // ✅ ADD THIS (error fix)
        public decimal DiscountAmount { get; set; }
    }
}