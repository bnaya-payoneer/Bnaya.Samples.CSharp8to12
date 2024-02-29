using System.Text;
using System.Text.Json;

using Bnaya.Samples;

using Microsoft.Extensions.Compliance.Redaction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDataProtection();


// credit: https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/
// 👇 Enable redaction of `[LogProperties]` objects
builder.Logging
    .ClearProviders()
    .AddJsonConsole(o =>
    {        
        o.JsonWriterOptions = new JsonWriterOptions
        {
            Indented = true,
        };
    });

builder.Services.AddRedaction(redactionBuilder =>
{
    //  Enable the erasing redactor for sensitive data
    redactionBuilder.SetRedactor<ErasingRedactor>(DataClassifications.Sensitive);

    redactionBuilder.SetHmacRedactor(hmacOpts =>
    {
        // ⚠ Don't do this in a real project - you need to load these values
        // from an options secret!
        hmacOpts.Key = "BLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAH";
        hmacOpts.KeyId = 123;
    }, DataClassifications.Personal);

    redactionBuilder.SetFallbackRedactor<CustomReductor>();
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
