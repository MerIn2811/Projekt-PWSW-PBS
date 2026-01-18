using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

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
            throw new Exception($"LOG IN HTTP {(int)resp.StatusCode}: {text}");

        return System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(
            text,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<GoalsResponse> GoalsAsync()
    {
        using var resp = await _http.GetAsync("getGoals");
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"GOALS HTTP {(int)resp.StatusCode}: {text}");

        try
        {
            var obj = JsonSerializer.Deserialize<GoalsResponse>(
                text,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (obj == null)
                throw new Exception("Deserialize returned null. Raw JSON: " + text);

            return obj;
        }
        catch (JsonException je)
        {
            throw new Exception("JSON parse error: " + je.Message + "\nRaw: " + text);
        }
    }

    public async Task<TasksResponse> TasksAsync(int goalId)
    {
        
        using var resp = await _http.GetAsync($"getTasks?goalId={goalId}");
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"TASKS HTTP {(int)resp.StatusCode}: {text}");

        return JsonSerializer.Deserialize<TasksResponse>(
            text,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task<SessionCheckResponse> CheckSessionAsync()
    {
        using var resp = await _http.GetAsync("checker"); // GET
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"SESSION CHECK HTTP {(int)resp.StatusCode}: {text}");

        return JsonSerializer.Deserialize<SessionCheckResponse>(
            text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    
}