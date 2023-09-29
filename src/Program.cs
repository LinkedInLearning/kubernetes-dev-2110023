using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(12);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));


builder.Services.AddRateLimiter(_ => _
    .AddSlidingWindowLimiter(policyName: "sliding", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(30);
        options.SegmentsPerWindow = 3;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

builder.Services.AddRateLimiter(_ => _
    .AddTokenBucketLimiter(policyName: "tokenPolicy", options =>
    {
        options.TokenLimit = 4;
        options.TokensPerPeriod = 30;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

builder.Services.AddRateLimiter(_ => _
    .AddConcurrencyLimiter(policyName: "concurrencyPolicy", options =>
    {
        options.PermitLimit = 4;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));


var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapHealthChecks("/health/startup");
app.MapHealthChecks("/healthz");
app.MapHealthChecks("/ready");

app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
