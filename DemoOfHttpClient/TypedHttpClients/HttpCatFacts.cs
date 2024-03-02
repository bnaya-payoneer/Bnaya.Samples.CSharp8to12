namespace DemoOfHttpClient;

public class HttpCatFacts
{
    private readonly HttpClient _client;

    public HttpCatFacts(HttpClient client)
    {
        _client = client;
    }

    public async Task<CatFact> GetCatFactAsync()
    {
        CatFact fact = await _client.GetFromJsonAsync<CatFact>("");
        return fact;    
    }
}