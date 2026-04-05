using System.ComponentModel.DataAnnotations;

namespace QuotationManagement.API.DTOs
{
    public class CreateQuoteDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        public DateTime QuoteDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal DiscountAmount { get; set; }
    }
}
