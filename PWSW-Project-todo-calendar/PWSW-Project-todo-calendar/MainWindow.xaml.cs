using System.Windows;
using PWSW_Project_todo_calendar.Pages;

namespace PWSW_Project_todo_calendar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new HomePage());
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
    }
}