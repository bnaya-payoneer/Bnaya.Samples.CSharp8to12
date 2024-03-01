using HealthChecks.UI.Client;
using HealthSample.Jobs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<MigrationJob>();

var cfg = builder.Configuration;
builder.Services.AddHealthChecks()
                .AddSqlServer(
                        cfg.GetConnectionString("SqlServerConnection")!,
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["persist", "live"])
                .AddSqlServer(
                        cfg.GetConnectionString("SqlServerConnection")!,
                        healthQuery: 
                            """
                            SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
                            WHERE TABLE_NAME = 'Todos' 
                            """,
                        name: "sql-server-schema",
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["persist", "ready"])
                .AddRedis(
                        cfg.GetConnectionString("RedisConnection")!,
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["persist", "cache", "live"]);
builder.Services.AddHealthChecksUI(o =>
                {
                    o.SetEvaluationTimeInSeconds(5); //time in seconds between check    
                    o.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                    //o.SetApiMaxActiveRequests(1); //api requests concurrency    
                    o.AddHealthCheckEndpoint("health api", "/health"); //map health check api    
                })
                .AddInMemoryStorage();


var app = builder.Build();

//app.MapHealthChecks("/health", new HealthCheckOptions 
//                            { 
//                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse                                
//                            })
//        .WithTags("live");
app.UseHealthChecksPrometheusExporter("/health-metrics");
app.UseRouting()
    .UseEndpoints(o =>
    {
        // liveness
        o.MapHealthChecks("/health/liveness", new HealthCheckOptions
        {
            Predicate = c => c.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse            
        });
        // readiness
        o.MapHealthChecks("/health/readiness", new HealthCheckOptions
        {
            Predicate = c => c.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse            
        });

        // health-ui
        o.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse            
        });
        o.MapHealthChecksUI(o =>
        {
            o.PageTitle = "Health Monitoring";
            o.UIPath = "/health-ui";
            o.AsideMenuOpened = true;
        });
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
