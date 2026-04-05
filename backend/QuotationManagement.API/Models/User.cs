namespace QuotationManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;   // plain for now (we'll improve later)

        public string Role { get; set; } = string.Empty;       // SalesRep, SalesManager, Admin
    }

}
