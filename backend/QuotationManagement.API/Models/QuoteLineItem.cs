namespace QuotationManagement.API.Models
{
    public class QuoteLineItem
    {
        public int Id { get; set; }

        public int QuoteId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        // ✅ ADD THIS (CRITICAL)
        public decimal LineTotal { get; set; }
    }
}