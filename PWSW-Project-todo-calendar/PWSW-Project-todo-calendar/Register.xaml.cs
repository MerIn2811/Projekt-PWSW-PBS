using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar;

public partial class Register : Window
{
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    private string? _avatarPath;
    public Register()
    {
        InitializeComponent();
    }

    private void Login_Window_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow();
        loginWindow.Show();
        Close();
    }

    private async void Register_Click(object sender, RoutedEventArgs e)
    {
        var mail = MailBox.Text?.Trim() ?? "";
        var username = UsernameBox.Text?.Trim() ?? "";
        var pass1 = PasswordBox.Password ?? "";
        var pass2 = PasswordBoxAgain.Password ?? "";

        if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pass1))
        {
            MessageBox.Show("Uzupełnij email, login i hasło.");
            return;
        }
        
        if (pass1.Length < 8)
        {
            MessageBox.Show("Hasło musi mieć min. 8 znaków.");
            return;
        }

        if (pass1 != pass2)
        {
            MessageBox.Show("Hasła nie są takie same.");
            return;
        }

        try
        {
            string avatarUrl = "";
            if (!string.IsNullOrWhiteSpace(_avatarPath))
            {
                avatarUrl = await _api.UploadAvatarAsync(_avatarPath);
            }

            await _api.RegisterAsync(mail, pass1, username, avatarUrl);
            MessageBox.Show("Konto utworzone! Możesz się zalogować.");

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Błąd rejestracji:\n" + ex.Message);
        }
    }
    
    private void ChooseImage_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Wybierz avatar",
            Filter = "Obrazy (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp|Wszystkie pliki (*.*)|*.*"
        };

        if (dlg.ShowDialog() == true)
        {
            _avatarPath = dlg.FileName;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(dlg.FileName);
            bmp.EndInit();
            bmp.Freeze();

            AvatarBrush.ImageSource = bmp;
        }
    }
}