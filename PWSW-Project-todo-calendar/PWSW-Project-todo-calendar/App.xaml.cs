using System.Windows;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar
{
    public partial class App : Application
    {
        private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var login = new LoginWindow();
            login.Show();
            
        }
    }
}