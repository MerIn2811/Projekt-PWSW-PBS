using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Models;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class ChangeGoalControl : System.Windows.Controls.UserControl
{
    private readonly int _goalId;
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    
    public ChangeGoalControl(int goalId)
    {
        _goalId = goalId;
        InitializeComponent();
    }


    private void GoToAdTask(object sender, RoutedEventArgs routedEventArgs)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new AddTaskPage(_goalId));
    }

    private async void DeleteGoal(object sender, RoutedEventArgs e)
    {
        try
        {
            _api.SetToken(UserSession.Token);

            await _api.DeleteGoalAsync(_goalId);


            UserSession.Goals = (await _api.GoalsAsync()).getGoals;
            
            var nav = NavigationService.GetNavigationService(this);
            nav?.Navigate(new HomePage());
            
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się usunąć goal:\n" + ex.Message);
        }
    }
}