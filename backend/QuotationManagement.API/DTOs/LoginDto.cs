using System.ComponentModel.DataAnnotations;

namespace QuotationManagement.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        public string Password { get; set; } = string.Empty;
    }
}
