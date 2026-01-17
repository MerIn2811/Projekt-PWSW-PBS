using System.Windows;
using System.Windows.Controls;

namespace PWSW_Project_todo_calendar.Pages
{
    public partial class AddGoalPage : Page
    {
        public AddGoalPage()
        {
            InitializeComponent();
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                ComboBox1.Items.Add(TextBox1.Text);
                TextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Wpisz tekst do dodania.");
            }
        }

        private void AskAboutTasks(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy chcesz od razu dodać zadania do twojego celu?",
                "Zawsze możesz dodać je później",
                MessageBoxButton.YesNo,
                MessageBoxImage.None);

            if (result == MessageBoxResult.Yes)
            {
                //Dodać logikę dodawania Goal do bazy i przekazać id Goal żeby dodać d oneigo task <3
                NavigationService.Navigate(new AddTaskPage());
            }
            
            if (result == MessageBoxResult.No)
            {
                NavigationService.Navigate(new HomePage());
            }
            
        }

       
    }
}