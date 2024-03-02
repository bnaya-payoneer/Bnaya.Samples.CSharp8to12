using DemoOfHttpClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// credit: https://apipheny.io/free-api/#apis-without-key
// https://devblogs.microsoft.com/dotnet/dotnet-8-networking-improvements/
services.ConfigureHttpClientDefaults(b =>
            {
                b.ConfigureHttpClient(cfg =>
                {
                    cfg.DefaultRequestHeaders.Add("Accept", "application/json");
                    cfg.DefaultRequestHeaders.Add("Demo", "true");
                });
            });

//// both clients will have MyAuthHandler added by default
services.AddHttpClient();
services.AddHttpClient("cat-facts", c => c.BaseAddress = new Uri("https://catfact.ninja/fact"));
services.AddHttpClient<HttpCatFacts>( c => c.BaseAddress = new Uri("https://catfact.ninja/fact"));
services.AddHttpClient("public-data", c => c.BaseAddress = new Uri("https://datausa.io/api/data"));


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

// ?drilldowns=Nation&measures=Population
