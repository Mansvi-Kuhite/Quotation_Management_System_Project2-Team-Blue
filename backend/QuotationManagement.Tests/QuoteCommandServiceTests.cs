using Xunit;
using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.CQRS.Handlers;
using QuotationManagement.API.Data;
using QuotationManagement.API.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class QuoteCommandServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // ---------------- CREATE TESTS ----------------

    [Fact]
    public async Task CreateQuote_Should_Set_Status_Draft()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var result = await service.CreateQuote(new Quote(), "salesrep");

        Assert.Equal("Draft", result.Status);
    }

    [Fact]
    public async Task CreateQuote_Should_Set_CreatedBy()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var result = await service.CreateQuote(new Quote(), "salesrep");

        Assert.Equal("salesrep", result.CreatedBy);
    }

    [Fact]
    public async Task CreateQuote_Should_Set_ExpiryDate()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var result = await service.CreateQuote(new Quote(), "salesrep");

        Assert.True(result.ExpiryDate > DateTime.UtcNow);
    }

    // ---------------- STATUS TESTS ----------------

    [Fact]
    public async Task Draft_To_Sent_Should_Work()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        var result = await service.ChangeStatus(quote.QuoteId, "Sent");

        Assert.Equal("Sent", result);
    }

    [Fact]
    public async Task Sent_To_Accepted_Should_Work()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");
        var result = await service.ChangeStatus(quote.QuoteId, "Accepted");

        Assert.Equal("Accepted", result);
    }

    [Fact]
    public async Task Draft_To_Accepted_Should_Fail()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await Assert.ThrowsAsync<Exception>(() =>
            service.ChangeStatus(quote.QuoteId, "Accepted"));
    }

    [Fact]
    public async Task Sent_To_Draft_Should_Fail()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");

        await Assert.ThrowsAsync<Exception>(() =>
            service.ChangeStatus(quote.QuoteId, "Draft"));
    }

    [Fact]
    public async Task Cannot_Change_After_Accepted()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");
        await service.ChangeStatus(quote.QuoteId, "Accepted");

        await Assert.ThrowsAsync<Exception>(() =>
            service.ChangeStatus(quote.QuoteId, "Sent"));
    }

    // ---------------- ADD ITEM TESTS ----------------

    [Fact]
    public async Task AddLineItem_Should_Work_For_Draft()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.AddLineItem(quote.QuoteId, new QuoteLineItem
        {
            ProductName = "Test",
            Quantity = 1,
            UnitPrice = 100
        });

        var saved = context.Quotes.Include(q => q.LineItems).First();
        Assert.Single(saved.LineItems);
    }

    [Fact]
    public async Task AddLineItem_Should_Work_For_Sent()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");

        await service.AddLineItem(quote.QuoteId, new QuoteLineItem());

        var saved = context.Quotes.Include(q => q.LineItems).First();
        Assert.Single(saved.LineItems);
    }

    [Fact]
    public async Task AddLineItem_Should_Fail_For_Accepted()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");
        await service.ChangeStatus(quote.QuoteId, "Accepted");

        await Assert.ThrowsAsync<Exception>(() =>
            service.AddLineItem(quote.QuoteId, new QuoteLineItem()));
    }

    // ---------------- DELETE TESTS ----------------

    [Fact]
    public async Task DeleteQuote_Should_Work_For_Draft()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.DeleteQuote(quote.QuoteId);

        Assert.Empty(context.Quotes);
    }

    [Fact]
    public async Task DeleteQuote_Should_Work_For_Sent()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");

        await service.DeleteQuote(quote.QuoteId);

        Assert.Empty(context.Quotes);
    }

    [Fact]
    public async Task DeleteQuote_Should_Fail_For_Accepted()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");
        await service.ChangeStatus(quote.QuoteId, "Accepted");

        await Assert.ThrowsAsync<Exception>(() =>
            service.DeleteQuote(quote.QuoteId));
    }

    // ---------------- INVALID ID TESTS ----------------

    [Fact]
    public async Task ChangeStatus_Invalid_Id_Should_Fail()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        await Assert.ThrowsAsync<Exception>(() =>
            service.ChangeStatus(999, "Sent"));
    }

    [Fact]
    public async Task AddLineItem_Invalid_Id_Should_Fail()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        await Assert.ThrowsAsync<Exception>(() =>
            service.AddLineItem(999, new QuoteLineItem()));
    }

    [Fact]
    public async Task DeleteQuote_Invalid_Id_Should_Fail()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        await Assert.ThrowsAsync<Exception>(() =>
            service.DeleteQuote(999));
    }

    // ---------------- EXTRA COVERAGE ----------------

    [Fact]
    public async Task Multiple_LineItems_Should_Be_Added()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.AddLineItem(quote.QuoteId, new QuoteLineItem());
        await service.AddLineItem(quote.QuoteId, new QuoteLineItem());

        var saved = context.Quotes.Include(q => q.LineItems).First();

        Assert.Equal(2, saved.LineItems.Count);
    }

    [Fact]
    public async Task Status_Should_Remain_Draft_After_Create()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        Assert.Equal("Draft", quote.Status);
    }

    [Fact]
    public async Task Cannot_Delete_After_Accepted_Even_If_Attempted_Twice()
    {
        var context = GetDbContext();
        var service = new QuoteCommandService(context);

        var quote = await service.CreateQuote(new Quote(), "rep");

        await service.ChangeStatus(quote.QuoteId, "Sent");
        await service.ChangeStatus(quote.QuoteId, "Accepted");

        await Assert.ThrowsAsync<Exception>(() =>
            service.DeleteQuote(quote.QuoteId));
    }
}