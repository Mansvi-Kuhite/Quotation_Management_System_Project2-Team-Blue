using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace QuotationManagement.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // For now just pass request
            await _next(context);
        }
    }
}