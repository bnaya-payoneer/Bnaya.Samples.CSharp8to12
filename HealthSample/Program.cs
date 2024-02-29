using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
                .AddSqlServer(
                        builder.Configuration.GetConnectionString("SqlServerConnection")!,
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["persist"],
                        name: "sql-server" )
                .AddRedis(
                        builder.Configuration.GetConnectionString("RedisConnection")!,
                        timeout: TimeSpan.FromSeconds(3),
                        tags: ["persist", "cache"]);
builder.Services.AddHealthChecksUI(opt =>
                {
                    opt.SetEvaluationTimeInSeconds(5); //time in seconds between check    
                    opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                    //opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                    opt.AddHealthCheckEndpoint("health api", "/health"); //map health check api    
                })
                .AddInMemoryStorage();


var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions 
                            { 
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse                                
                            });
app.UseHealthChecksPrometheusExporter("/health-metrics");
app.UseRouting()
    .UseEndpoints(cfg =>
                   cfg.MapHealthChecksUI(o =>
                   {
                       o.PageTitle = "Health Monitoring";
                       o.UIPath = "/health-ui";
                       o.AsideMenuOpened = true;
                   }));

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
