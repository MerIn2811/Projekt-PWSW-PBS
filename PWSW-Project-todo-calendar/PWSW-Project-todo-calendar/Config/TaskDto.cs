namespace PWSW_Project_todo_calendar.Config;

public sealed class TaskDto
{
    public long idTask { get; set; }
    public long idGoal { get; set; }
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public int importance { get; set; }
    public string endDate { get; set; } = "";
    public bool isFinished { get; set; }


}