using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PWSW_Project_todo_calendar.Pages;

public partial class MenuPage : Page
{
    public MenuPage()
    {
        InitializeComponent();
        
    }
    
   
    private void GoPassword_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Password));

    private void GoEmail_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Email));

    private void GoUsername_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Username));

    private void GoAvatar_Click(object sender, RoutedEventArgs e)
        => NavigationService?.Navigate(new SettingsEditPage(SettingsEditMode.Avatar));
    
    
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TextBox1.Text))
        {
            TextBox1.Clear();
        }
        else
        {
            MessageBox.Show("Wpisz tekst do dodania.",null, MessageBoxButton.OK, MessageBoxImage.Error );
        }
    }
    
    
}