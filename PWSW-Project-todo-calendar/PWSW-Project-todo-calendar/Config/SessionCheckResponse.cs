namespace PWSW_Project_todo_calendar.Config;

public sealed class SessionCheckResponse
{
    public bool valid { get; set; }
    public string reason { get; set; } = "";
    public string expiresAt { get; set; } = "";
    public UserDto? user { get; set; }
}