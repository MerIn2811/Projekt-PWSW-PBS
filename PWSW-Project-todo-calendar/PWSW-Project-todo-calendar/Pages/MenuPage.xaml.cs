using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar.Pages;

public partial class MenuPage : Page
{
    public MenuPage()
    {
        InitializeComponent();
        AvatarImage.ImageSource = new BitmapImage(
            new Uri(UserSession.Avatar));
    }
    
   
    private void GoPassword_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Password, UserSession.UserId));

    private void GoEmail_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Email, UserSession.UserId));

    private void GoUsername_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Username, UserSession.UserId));

    private void GoAvatar_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Avatar,  UserSession.UserId));
    
    
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
    
    
}