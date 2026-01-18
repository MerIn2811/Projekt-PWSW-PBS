namespace PWSW_Project_todo_calendar.Config;

public static class UserSession
{
    public static string Token { get; set; } = "";
    public static int UserId { get; set; }
    public static string Username { get; set; } = "";
    public static string Mail { get; set; } = "";
    public static string Avatar { get; set; } = "";

    public static List<GoalDto> Goals { get; set; } = new();
    public static Dictionary<int, List<TaskDto>> TasksByGoal { get; set; } = new();
    
    public static void ClearCache()
    {
        Goals = new();
        TasksByGoal = new();
    }
}