using System.ComponentModel.DataAnnotations;

namespace QuotationManagement.API.DTOs
{
    public class CreateLineItemDto
    {
        [Required]
        [MinLength(2)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal UnitPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal Discount { get; set; }
    }
}
