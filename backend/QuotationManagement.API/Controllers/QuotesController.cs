using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuotationManagement.API.CQRS.Handlers;
using QuotationManagement.API.DTOs;
using QuotationManagement.API.Models;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly QuoteCommandService _commandService;
    private readonly QuoteQueryHandler _queryHandler;
    private readonly ILogger<QuotesController> _logger;

    public QuotesController(
        QuoteCommandService commandService,
        QuoteQueryHandler queryHandler,
        ILogger<QuotesController> logger)
    {
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var data = await _queryHandler.GetAllQuotes();
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "SalesRep")
            {
                data = data.Where(q => q.CreatedBy == username).ToList();
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch quotes for user {Username}", User.Identity?.Name);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Failed to load quotes.",
                    Detail = "An error occurred while fetching quotes."
                });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var data = await _queryHandler.GetQuoteById(id);
        return data is null ? NotFound("Quote not found.") : Ok(data);
    }

    [HttpPost]
    [Authorize(Roles = "SalesRep")]
    public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (dto.QuoteDate == default || dto.ExpiryDate == default)
        {
            return BadRequest("Quote date and expiry date are required.");
        }

        if (dto.ExpiryDate < dto.QuoteDate)
        {
            return BadRequest("Expiry date must be greater than or equal to quote date.");
        }

        var username = User.Identity?.Name ?? "system";
        var quote = new Quote
        {
            CustomerId = dto.CustomerId,
            QuoteDate = dto.QuoteDate,
            ExpiryDate = dto.ExpiryDate,
            DiscountAmount = dto.DiscountAmount
        };

        var result = await _commandService.CreateQuote(quote, username);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _commandService.DeleteQuote(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/items")]
    public async Task<IActionResult> AddItem(int id, [FromBody] CreateLineItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var item = new QuoteLineItem
        {
            ProductName = dto.ProductName,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            Discount = dto.Discount
        };

        try
        {
            await _commandService.AddLineItem(id, item);
            return Ok();
        }
        catch (Exception ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "SalesManager,Admin")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return BadRequest("Status is required.");
        }

        try
        {
            await _commandService.ChangeStatus(id, status.Trim());
            return Ok("Status updated successfully");
        }
        catch (Exception ex)
        {
            return ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(ex.Message)
                : BadRequest(ex.Message);
        }
    }

    [HttpGet("analytics")]
    [Authorize(Roles = "SalesManager,Admin")]
    public async Task<IActionResult> GetAnalytics()
    {
        var data = await _queryHandler.GetAnalytics();
        return Ok(data);
    }
}
