using System.Text.Json.Serialization;

namespace DemoOfHttpClient;

public readonly record struct NationPopulation()
{
    [JsonPropertyName("ID Nation")]
    public string IDNation { get; }
    public string Nation { get; }
    [JsonPropertyName("ID Year")]
    public int IDYear { get; }
    public string Year { get; }
    public string Population { get; }
    [JsonPropertyName("Slug Nation")]
    public string SlugNation { get; }
}