
using System.Net.Http;
using System.Text.Json;

namespace PWSW_Project_todo_calendar.Config;

public class ApiService
{
    private readonly HttpClient _client = new();

    public async Task<List<TaskDto>> GetTasksByGoalAsync(int goalId)
    {
        var url = $"https://twojastrona.pl/api/getTasksByGoal.php?goalId={goalId}";
        var json = await _client.GetStringAsync(url);

        return JsonSerializer.Deserialize<List<TaskDto>>(json)!;
    }
    
    private async Task<List<GoalDto>> GetGoalsAsync(int goalId)
    {
        using var client = new HttpClient();

        var url = $"https://twojastrona.pl/api/getTasksByGoal.php?goalId={goalId}\n";
        var json = await client.GetStringAsync(url);

        return JsonSerializer.Deserialize<List<GoalDto>>(json)!;
    }
}