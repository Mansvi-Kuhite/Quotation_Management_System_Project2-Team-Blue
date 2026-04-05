namespace QuotationManagement.API.DTOs
{
    public class AnalyticsDto
    {
        public int TotalQuotes { get; set; }
        public int AcceptedQuotes { get; set; }
        public int RejectedQuotes { get; set; }
        public decimal TotalRevenue { get; set; }
        public double ConversionRate { get; set; }
    }
}