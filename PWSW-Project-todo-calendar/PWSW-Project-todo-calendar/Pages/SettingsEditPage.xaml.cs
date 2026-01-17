using System.Windows;
using System.Windows.Controls;
using PWSW_Project_todo_calendar.Pages.UserControl;

namespace PWSW_Project_todo_calendar.Pages;

public partial class SettingsEditPage : Page
{
    private readonly SettingsEditMode _mode;

    public SettingsEditPage(SettingsEditMode mode)
    {
        InitializeComponent();
        _mode = mode;

        TitleText.Text = mode switch
        {
            SettingsEditMode.Password => "Zmień hasło",
            SettingsEditMode.Email    => "Zmień maila",
            SettingsEditMode.Username => "Zmień nazwę użytkownika",
            SettingsEditMode.Avatar   => "Zmień obraz użytkownika",
            _ => "Ustawienia"
        };

        ContentHost.Content = mode switch
        {
            SettingsEditMode.Password => new ChangePasswordContent(),
            SettingsEditMode.Email    => new ChangeEmailContent(),
            SettingsEditMode.Avatar   => new ChangeAvatarContent(),
            SettingsEditMode.Goal => new ChangeGoalControl(),
            SettingsEditMode.Task => new ChangeTaskControl(),
            _ => new TextBlock { Text = "Brak widoku." }
        };
    }

    private void Back_Click(object sender, RoutedEventArgs e)
        => NavigationService?.GoBack();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => NavigationService?.GoBack();

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show("Zapisano (demo).");
        if (result == MessageBoxResult.OK)
        {
            NavigationService?.GoBack();
        }
        
    }
}