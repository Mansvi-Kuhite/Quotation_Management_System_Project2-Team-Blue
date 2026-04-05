using Microsoft.AspNetCore.Mvc;
using QuotationManagement.API.CQRS.Handlers;
using QuotationManagement.API.Models;
using QuotationManagement.API.DTOs;

namespace QuotationManagement.API.Controllers
{
    [ApiController]
    [Route("api/quotes")]
    public class QuotesController : ControllerBase
    {
        private readonly QuoteCommandService _commandService;
        private readonly QuoteQueryHandler _queryHandler;
        private readonly QuoteQueryService _queryService;

        public QuotesController(
            QuoteCommandService commandService,
            QuoteQueryHandler queryHandler,
            QuoteQueryService queryService)
        {
            _commandService = commandService;
            _queryHandler = queryHandler;
            _queryService = queryService;
        }

        // ✅ GET ALL QUOTES
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _queryHandler.GetAllQuotes();
            return Ok(data);
        }

        // ✅ GET QUOTE BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _queryHandler.GetQuoteById(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // ✅ CREATE QUOTE (USING DTO)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
        {
            var quote = new Quote
            {
                CustomerId = dto.CustomerId,
                DiscountAmount = dto.DiscountAmount
            };

            var result = await _commandService.CreateQuote(quote);
            return Ok(result);
        }

        // ✅ DELETE QUOTE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _commandService.DeleteQuote(id);
            return Ok();
        }

        // ✅ ADD LINE ITEM (USING DTO)
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddItem(int id, [FromBody] CreateLineItemDto dto)
        {
            var item = new QuoteLineItem
            {
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Discount = dto.Discount
            };

            await _commandService.AddLineItem(id, item);
            return Ok();
        }

        // ✅ CHANGE STATUS
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] string status)
        {
            await _commandService.ChangeStatus(id, status);
            return Ok("Status updated successfully");
        }

        // ✅ ANALYTICS
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var data = await _queryHandler.GetAnalytics();
            return Ok(data);
        }
    }
}