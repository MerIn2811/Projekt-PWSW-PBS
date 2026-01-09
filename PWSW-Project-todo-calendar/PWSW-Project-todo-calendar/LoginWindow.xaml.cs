using System.Windows;
using PWSW_Project_todo_calendar.Models;

namespace PWSW_Project_todo_calendar
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            
            var username = UsernameBox.Text?.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Podaj login.");
                return;
            }

            
            var user = new User { Username = username };

            var main = new MainWindow();
            main.Show();

            this.Close();
        }
    }
}