using System.Net.Http;
using System.Net.Http.Json;

namespace PWSW_Project_todo_calendar.Config;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public void SetToken(string token)
    {
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<LoginResponse> LoginAsync(string login, string password)
    {
        var payload = new { login, password };

        using var resp = await _http.PostAsJsonAsync("login", payload);
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"HTTP {(int)resp.StatusCode}: {text}");

        return System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(
            text,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
}