namespace QuotationManagement.API.CQRS.Commands
{
    public class CreateQuoteCommand
    {
        public int CustomerId { get; set; }

        public DateTime QuoteDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty; // ✅ FIX
    }
}