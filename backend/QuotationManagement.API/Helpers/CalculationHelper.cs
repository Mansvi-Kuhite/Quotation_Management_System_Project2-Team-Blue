using QuotationManagement.API.Models;
using System.Linq;

namespace QuotationManagement.API.Helpers
{
    public static class CalculationHelper
    {
        public static void CalculateTotals(Quote quote)
        {
            // ✅ SubTotal
            quote.SubTotal = quote.LineItems.Sum(x => x.LineTotal);

            // ✅ Tax (example 18%)
            quote.TaxAmount = quote.SubTotal * 0.18m;

            // ✅ Grand Total
            quote.GrandTotal = quote.SubTotal + quote.TaxAmount - quote.DiscountAmount;
        }
    }
}