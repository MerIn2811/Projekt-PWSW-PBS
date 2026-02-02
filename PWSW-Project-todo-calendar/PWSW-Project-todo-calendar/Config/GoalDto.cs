namespace PWSW_Project_todo_calendar.Config;

public sealed class GoalDto
{
    public int idGoal { get; set; }
    public string name { get; set; } = "";
    public string category { get; set; } = "";
    public int importance { get; set; }
    public bool isFinished { get; set; }
    public string? description { get; set; } = "";
    public string? startDate { get; set; } = "";
    public string? endDate { get; set; } = "";
    
    
}