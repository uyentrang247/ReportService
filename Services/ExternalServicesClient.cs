using System.Net.Http.Json;
using ReportService.Models;

namespace ReportService.Services;

public class ExternalServicesClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalServicesClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task<List<ExternalProductDto>?> GetProductsAsync(string? token)
        => GetAsync<List<ExternalProductDto>>("ProductService", "/products", token);

    public Task<ExternalProductDto?> GetProductAsync(int id, string? token)
        => GetAsync<ExternalProductDto>("ProductService", $"/products/{id}", token);

    public Task<List<ExternalOrderDto>?> GetOrdersAsync(string? token)
        => GetAsync<List<ExternalOrderDto>>("OrderService", "/orders", token);

    public Task<ExternalOrderDto?> GetOrderAsync(int id, string? token)
        => GetAsync<ExternalOrderDto>("OrderService", $"/orders/{id}", token);

    private async Task<T?> GetAsync<T>(string clientName, string path, string? token)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(clientName);
            using var req = new HttpRequestMessage(HttpMethod.Get, path);
            if (!string.IsNullOrEmpty(token))
                req.Headers.TryAddWithoutValidation("Authorization", token);

            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
                return default;

            return await res.Content.ReadFromJsonAsync<T>();
        }
        catch
        {
            return default;
        }
    }
}
