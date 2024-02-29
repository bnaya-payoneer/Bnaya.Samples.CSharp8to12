using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cfg = builder.Configuration;
builder.Services.AddHealthChecks()
                .AddSqlServer(cfg.GetConnectionString("SqlServerConnection")!)
                .AddRedis(cfg.GetConnectionString("RedisConnection")!);
builder.Services.AddHealthChecksUI(o =>
                {    
                    o.AddHealthCheckEndpoint("health api", "/health");    
                })
                .AddInMemoryStorage();


var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions 
                            { 
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse                                
                            });
app.UseHealthChecksPrometheusExporter("/health-metrics");
app.UseRouting()
    .UseEndpoints(o => o.MapHealthChecksUI(o =>o.UIPath = "/health-ui"));

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
