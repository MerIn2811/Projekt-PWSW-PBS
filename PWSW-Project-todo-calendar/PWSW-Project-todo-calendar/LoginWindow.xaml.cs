using System;
using System.Windows;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar
{
    public partial class LoginWindow : Window
    {
        private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = UsernameBox.Text?.Trim() ?? "";
            var password = PasswordBox.Password ?? "";

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Podaj login i hasło.");
                return;
            }

            try
            {
                var res = await _api.LoginAsync(login, password);

                UserSession.Token = res.token;
                UserSession.UserId = res.user.idUser;
                UserSession.Username = res.user.username;

                _api.SetToken(res.token);

                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd logowania:\n" + ex.Message);
            }
        }
    }
}