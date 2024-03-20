using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Caching;
using Polly.Caching.Distributed;
using Polly.Registry;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(action => {
    var connection = "localhost:6379"; //the REDIS connection
    action.Configuration = connection;
});

builder.Services.AddResiliencePipeline("my-pipeline", builder =>
{
    builder
        .AddRetry(new RetryStrategyOptions())
        .AddTimeout(TimeSpan.FromSeconds(10));
});

builder.Services.AddSingleton(serviceProvider =>
{
    IDistributedCache distrebutedCache = serviceProvider.GetRequiredService<IDistributedCache>();
    IAsyncCacheProvider<DateTimeOffset> result = distrebutedCache.AsAsyncCacheProvider<DateTimeOffset>();
    return result;
});

builder.Services.AddSingleton((serviceProvider) =>
{
    PolicyRegistry registry = new PolicyRegistry();
    IAsyncCacheProvider<DateTimeOffset> policy = serviceProvider.GetRequiredService<IAsyncCacheProvider<DateTimeOffset>>();
    registry.Add<IAsyncPolicy<DateTimeOffset>>("py-cache",
        Policy.CacheAsync(policy, TimeSpan.FromMinutes(5)));

    return registry;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
