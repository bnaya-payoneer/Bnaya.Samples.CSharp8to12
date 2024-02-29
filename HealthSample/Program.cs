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

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions 
                            { 
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse                                
                            });

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
