using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class TaskRowControl : System.Windows.Controls.UserControl
{
    private readonly int _taskId;
    private readonly int _goal;
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    private readonly Func<Task> _refreshGoal;
    
    public TaskRowControl(TaskDto task, int goalId, Func<Task> refreshGoal)
    {
        InitializeComponent();
        _taskId = (int)task.idTask;
        _goal = goalId;
        
        _refreshGoal = refreshGoal;
        
        if (task.isFinished == true) TaskIsFinished.IsChecked = true;
        else TaskIsFinished.IsChecked = false;
        
        TaskName.Text = task.name;
        EndDateTask.Text = FormatDate(task.endDate);

        switch (task.importance)
        {
            case 0:
                Colour.Background = new SolidColorBrush(Colors .Pink);
                break;
            case 1:
                Colour.Background = new SolidColorBrush(Colors.DarkSalmon);
                break;
            case 2:
                Colour.Background = new SolidColorBrush(Colors.OrangeRed);
                break;
            default:
                Colour.Background = new SolidColorBrush(Colors.White);
                break;
        }
        
    }

    private void goToTaskChangeClick(object sender, RoutedEventArgs routedEventArgs)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new SettingsEditPage(SettingsEditMode.Task));
    }
    
    private string FormatDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        if (DateTime.TryParse(value, out var dt))
            return dt.ToString("dd.MM.yyyy HH:mm");

        return value;
    }
    

    private async void Checked(object sender, RoutedEventArgs e)
    {
            try
            {
                var isFinished = TaskIsFinished.IsChecked == true ? 1 : 0;
                _api.SetToken(UserSession.Token);

                await _api.PatchTaskAsyncIsFinished(_taskId, new
                {
                    isFinished = isFinished
                });

                var tasksRes = await _api.TasksAsync(_goal);
                UserSession.TasksByGoal[_goal] = tasksRes.tasks;
                
                await _refreshGoal();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się zaktualizować goal:\n" + ex.Message);
            }
    }
}