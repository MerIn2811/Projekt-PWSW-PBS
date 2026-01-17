using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class GoalRowControl : System.Windows.Controls.UserControl
{
    public GoalRowControl()
    {
        InitializeComponent();
    }
    
    private void ToggleTask_Checked(object sender, RoutedEventArgs e)
    {
        TasksPanel.Visibility = Visibility.Visible;
        ToggleTask.Content = "▲";
    }

    private void ToggleTask_Unchecked(object sender, RoutedEventArgs e)
    {
        TasksPanel.Visibility = Visibility.Collapsed;
        ToggleTask.Content = "▼";
    }

    private void goGoalControlClick(object sender, RoutedEventArgs e)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new SettingsEditPage(SettingsEditMode.Goal));
    }
}