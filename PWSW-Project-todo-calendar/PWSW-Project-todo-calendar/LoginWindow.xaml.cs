using System;
using System.Diagnostics;
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
            Loaded += LoginWindow_Loaded;
        }

        private async void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string savedToken = TokenStorage.Load();
            
            if (string.IsNullOrWhiteSpace(savedToken))
                return;

            try
            {
                _api.SetToken(savedToken);

                var check = await _api.CheckSessionAsync();
                if (!check.valid)
                {
                    TokenStorage.Clear();
                    UserSession.Token = "";
                    return;
                }

                if (check.valid && check.user != null)
                {
                    UserSession.Token = savedToken;
                    UserSession.UserId = (int)check.user.idUser;
                    UserSession.Username = check.user.username;
                    UserSession.Mail = check.user.mail;
                    UserSession.Avatar = check.user.avatar;
                    
                    var goalsRes = await _api.GoalsAsync();
                    UserSession.Goals = goalsRes.getGoals;

                    UserSession.TasksByGoal = new Dictionary<int, List<TaskDto>>();
                    foreach (var g in UserSession.Goals)
                    {
                        var tasksRes = await _api.TasksAsync(g.idGoal);
                        UserSession.TasksByGoal[g.idGoal] = tasksRes.tasks;
                    }

                    var main = new MainWindow();
                    main.Show();
                    Close();
                }
                
            }
            catch (Exception ex)
            {
                TokenStorage.Clear();
                UserSession.Token = "";
                MessageBox.Show("Check session error:\n" + ex.Message);
            }
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
                _api.SetToken(res.token);
                TokenStorage.Save(res.token);
                UserSession.UserId = (int)res.user.idUser;
                UserSession.Username = res.user.username;
                UserSession.Mail = res.user.mail;
                UserSession.Avatar = res.user.avatar;
                
                _api.SetToken(res.token);
                

                
                var goalsRes = await _api.GoalsAsync();
                UserSession.Goals = goalsRes.getGoals;
                

                
                UserSession.TasksByGoal = new Dictionary<int, List<TaskDto>>();
                foreach (var g in UserSession.Goals)
                {
                    var tasksRes = await _api.TasksAsync(g.idGoal);
                    UserSession.TasksByGoal[g.idGoal] = tasksRes.tasks;
                }
                
                
                
                var main = new MainWindow();
                main.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd logowania:\n" + ex.Message);
            }
            
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var register = new Register();
            register.Show();
            Close();
        }
    }
}