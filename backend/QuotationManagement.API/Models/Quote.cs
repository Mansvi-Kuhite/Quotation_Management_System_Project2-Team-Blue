using System;
using System.Collections.Generic;

namespace QuotationManagement.API.Models
{
    public class Quote
    {
        public int QuoteId { get; set; }

        public string QuoteNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }

        public DateTime QuoteDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        // ✅ ADD THESE (CRITICAL FIX)
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public int RevisionNumber { get; set; }

        public List<QuoteLineItem> LineItems { get; set; } = new();
    }
}