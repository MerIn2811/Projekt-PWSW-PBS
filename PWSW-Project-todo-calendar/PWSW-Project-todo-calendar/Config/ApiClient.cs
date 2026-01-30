using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
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
        using var resp = await _http.GetAsync("checker");
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"SESSION CHECK HTTP {(int)resp.StatusCode}: {text}");

        return JsonSerializer.Deserialize<SessionCheckResponse>(
            text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    
    public async Task LogoutAsync()
    {
        using var resp = await _http.PostAsync("logout", null);
        var text = await resp.Content.ReadAsStringAsync();
        
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"SESSION HTTP {(int)resp.StatusCode}: {text}");
    }

    public async Task<string> UploadAvatarAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();

        var bytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        form.Add(fileContent, "avatar", Path.GetFileName(filePath));

        using var resp = await _http.PostAsync("uploadAvatar", form);
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"UPLOAD HTTP {(int)resp.StatusCode}: {text}");

        using var doc = JsonDocument.Parse(text);
        return doc.RootElement.GetProperty("avatarUrl").GetString()!;
    }
    
    public async Task RegisterAsync(string mail, string password, string username, string avatarUrl)
    {
        using var resp = await _http.PostAsJsonAsync("register", new
        {
            username,
            mail,
            password,
            avatar = avatarUrl  
        });

        var text = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"REGISTER HTTP {(int)resp.StatusCode}: {text}");
    }

    public async Task UpdateAvatarAsync(string avatarUrl)
    {
        using var resp = await _http.PostAsJsonAsync("setAvatar", new { avatar = avatarUrl });

        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"UPDATE HTTP {(int)resp.StatusCode}\n{text}");
    }

    public async Task<int> AddGoalAsync(string name, DateTime endDate, string category, int importance, string description)
    {

        var payload = new
        {
            name,
            endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
            category,
            importance,
            description
        };

        using var resp = await _http.PostAsJsonAsync("AddGoal", payload);
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"ADD GOAL HTTP {(int)resp.StatusCode}: {text}");
        
        var obj = JsonSerializer.Deserialize<AddGoalResponse>(
            text,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        if (obj == null || obj.goalId <= 0)
            throw new Exception("Nie udało się odczytać ID nowego celu. Raw: " + text);

        return obj.goalId;
    }
    
    public async Task<int> AddTaskAsync(int goalId, string name, DateTime endDate, int importance, string description)
    {
        var payload = new
        {
            goalId,
            name,
            endDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
            importance,
            description
        };

        using var resp = await _http.PostAsJsonAsync("AddTask", payload);
        var text = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"ADD Task HTTP {(int)resp.StatusCode}: {text}");

        var obj = JsonSerializer.Deserialize<AddTaskResponse>(
            text,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (obj == null || obj.TaskId <= 0)
            throw new Exception("Nie udało się odczytać ID nowego taska. Raw: " + text);

        return obj.TaskId;
    }

    public async Task PatchGoalAsyncIsFinished(int goalId, object patch)
    {
        
        var payload = new Dictionary<string, object?>
        {
            ["goalId"] = goalId
        };

        
        var jsonPatch = JsonSerializer.Serialize(patch);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonPatch)
                   ?? new Dictionary<string, object?>();

        foreach (var kv in dict)
            payload[kv.Key] = kv.Value;

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var req = new HttpRequestMessage(new HttpMethod("PATCH"),"patchGoal")
        {
            Content = content
        };

        var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception(body);
    }
    
    public async Task PatchTaskAsyncIsFinished(int taskId, object patch)
    {
        
        var payload = new Dictionary<string, object?>
        {
            ["taskId"] = taskId
        };

        
        var jsonPatch = JsonSerializer.Serialize(patch);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonPatch)
                   ?? new Dictionary<string, object?>();

        foreach (var kv in dict)
            payload[kv.Key] = kv.Value;

        var json = JsonSerializer.Serialize(payload);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var req = new HttpRequestMessage(new HttpMethod("PATCH"),"patchTask")
        {
            Content = content
        };

        var res = await _http.SendAsync(req);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            throw new Exception(body);
    }

    public async Task<int> DeleteGoalAsync(int goalId)
    {
        var payload = new Dictionary<string, object?>
        {
            ["goalId"] = goalId
        };
        
        using var resp = await _http.PostAsJsonAsync("deleteGoal", payload);
        var text = await resp.Content.ReadAsStringAsync();
        
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"DELETE HTTP {(int)resp.StatusCode}: {text}");

        return goalId;
    }
    
    public async Task<int> DeleteTaskAsync(int taskId)
    {
        var payload = new Dictionary<string, object?>
        {
            ["taskId"] = taskId
        };
        
        using var resp = await _http.PostAsJsonAsync("deleteTask", payload);
        var text = await resp.Content.ReadAsStringAsync();
        
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"DELETE HTTP {(int)resp.StatusCode}: {text}");

        return taskId;
    }
    
}