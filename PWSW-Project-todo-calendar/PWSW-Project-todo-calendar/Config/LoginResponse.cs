namespace PWSW_Project_todo_calendar.Config;

public sealed class LoginResponse
{
    public string token { get; set; } = "";
    public UserDto user { get; set; } = new();
    public string expiresAt { get; set; } = "";
}