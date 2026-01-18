using System.Windows;
using System.Windows.Media.Imaging;
using OpenTK.Platform.Windows;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Pages;

namespace PWSW_Project_todo_calendar
{
    
    public partial class MainWindow : Window
    {
        private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
            AvatarBrush.ImageSource = new BitmapImage(
                new Uri(UserSession.Avatar));
            Username.Text = UserSession.Username;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }

        private void AddGoal_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddGoalPage());
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuPage());
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UserSession.Token))
                    _api.SetToken(UserSession.Token);

                await _api.LogoutAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                TokenStorage.Clear();
                UserSession.ClearAll();

                var login = new LoginWindow();
                login.Show();
                Close();
            }
        }
    }
}