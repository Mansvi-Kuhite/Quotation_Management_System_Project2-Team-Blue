using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using QuotationManagement.API.Data;
using QuotationManagement.API.CQRS.Handlers;
using QuotationManagement.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);

// Add CacheServices
builder.Services.AddSingleton<CacheServices>();

// Add handlers, services, DbContext, etc.
builder.Services.AddScoped<QuoteQueryHandler>();
builder.Services.AddScoped<QuoteCommandService>();

builder.Services.AddScoped<QuoteCommandService>();
builder.Services.AddScoped<QuoteQueryHandler>();
builder.Services.AddScoped<QuoteQueryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();