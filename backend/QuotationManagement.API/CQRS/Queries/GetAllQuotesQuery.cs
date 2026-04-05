namespace QuotationManagement.API.CQRS.Queries
{
    public class GetAllQuotesQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}