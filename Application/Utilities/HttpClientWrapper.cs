using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.Utilities;

public class HttpClientWrapper
{
    private static readonly HttpClient client = new HttpClient();

    public HttpClientWrapper(IConfiguration configuration)
    {
        client.BaseAddress = new Uri(configuration["ApiUrl"]!);
    }

    //After creation of logger, wrap this method in [Time] adnotation and insall package for method time logger
    public async Task<T?> Get<T>(string endpoint) where T : class
    {
        try
        {
            var response = await client.GetFromJsonAsync<T>($"{endpoint}");
            return response;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
