using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.Services.Base;

public abstract class ApiService
{
    protected readonly HttpClient _http;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    protected ApiService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(url, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(_json, ct);
    }

    protected async Task<T?> PostAsync<T>(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(url, body, _json, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(_json, ct);
    }

    protected async Task<T?> PutAsync<T>(string url, object body, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync(url, body, _json, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(_json, ct);
    }

    protected async Task HttpDeleteAsync(string url, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(url, ct);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var content = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(content, _json);
        throw new ApiException(error?.Error ?? "Erro inesperado.", response.StatusCode);
    }

    private record ErrorResponse(string Error);
}
