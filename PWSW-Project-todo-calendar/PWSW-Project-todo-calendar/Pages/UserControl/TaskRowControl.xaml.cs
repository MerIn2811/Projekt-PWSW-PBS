using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class TaskRowControl : System.Windows.Controls.UserControl
{
    public TaskRowControl()
    {
        InitializeComponent();
    }

    private void goToTaskChangeClick(object sender, RoutedEventArgs routedEventArgs)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new SettingsEditPage(SettingsEditMode.Task));
    }
}