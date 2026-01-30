using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Pages.UserControl;

namespace PWSW_Project_todo_calendar.Pages;

public partial class SettingsEditPage : Page
{
    private readonly SettingsEditMode _mode;
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    private ChangeAvatarContent? _avatarContent;
    
    public SettingsEditPage(SettingsEditMode mode, int idSomething)
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
            SettingsEditMode.Avatar   => _avatarContent = new ChangeAvatarContent(),
            SettingsEditMode.Goal => new ChangeGoalControl(idSomething),
            SettingsEditMode.Task => new ChangeTaskControl(idSomething),
            _ => new TextBlock { Text = "Brak widoku." }
        };
    }

    private void Back_Click(object sender, RoutedEventArgs e)
        => NavigationService?.GoBack();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => NavigationService?.GoBack();

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        
        switch (_mode)
        {
            case SettingsEditMode.Avatar:
                var path = _avatarContent.SelectedAvatarPath;


                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                try
                {

                    _api.SetToken(UserSession.Token);

                    var avatarUrl = await _api.UploadAvatarAsync(path);

                    await _api.UpdateAvatarAsync(avatarUrl);

                    UserSession.Avatar = avatarUrl;

                    MessageBox.Show("Avatar został zmieniony.", "OK");

                    NavigationService?.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd zapisu:\n" + ex.Message, "ERROR");
                }
                break;
            case SettingsEditMode.Password:
                break;
            case SettingsEditMode.Email:
                break;
            case SettingsEditMode.Username:
                break;
            case SettingsEditMode.Goal:
                break;
            case SettingsEditMode.Task:
                break;
            
        }

        var newMain = new MainWindow();
        Application.Current.MainWindow = newMain;
        newMain.Show();
        
        Window.GetWindow(this)?.Close();


    }
}