using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class ChangeTaskControl : System.Windows.Controls.UserControl
{
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    private readonly int _taskId;
    
    public ChangeTaskControl(int taskId)
    {
        InitializeComponent();
        _taskId = taskId;
    }

    private async void DeleteTask(object sender, RoutedEventArgs e)
    {
            try
            {
                _api.SetToken(UserSession.Token);

                await _api.DeleteTaskAsync(_taskId);


                UserSession.TasksByGoal = new Dictionary<int, List<TaskDto>>();
                foreach (var g in UserSession.Goals)
                {
                    var tasksRes = await _api.TasksAsync(g.idGoal);
                    UserSession.TasksByGoal[g.idGoal] = tasksRes.tasks;
                }
            
                var nav = NavigationService.GetNavigationService(this);
                nav?.Navigate(new HomePage());
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się usunąć goal:\n" + ex.Message);
            }
    }
}