using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class ChangeGoalControl : System.Windows.Controls.UserControl
{
    public ChangeGoalControl()
    {
        InitializeComponent();
    }


    private void GoToAdTask(object sender, RoutedEventArgs routedEventArgs)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new AddTaskPage(1));
    }
    
}