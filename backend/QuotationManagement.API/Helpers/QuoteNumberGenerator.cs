using System;

namespace QuotationManagement.API.Helpers
{
    public static class QuoteNumberGenerator
    {
        public static string Generate(int count)
            => $"QT-{DateTime.Now.Year}-{count.ToString("D4")}";
    }
}