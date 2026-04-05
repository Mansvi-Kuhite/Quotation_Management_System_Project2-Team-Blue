namespace QuotationManagement.API.DTOs
{
    public class CreateLineItemDto
    {
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        // ✅ FIX names to match controller
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}