using System.Windows;
using System.Windows.Controls;
using PWSW_Project_todo_calendar.Config;
using System.Windows.Documents;

namespace PWSW_Project_todo_calendar.Pages
{
    public partial class AddGoalPage : Page
    {
        
        private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
        
        public AddGoalPage()
        {
            InitializeComponent();
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                GoalCat.Items.Add(TextBox1.Text);
                TextBox1.Clear();
            }
            else
            {
                MessageBox.Show("Wpisz tekst do dodania.");
            }
        }

        private async void AskAboutTasks(object sender, RoutedEventArgs e)
        {
            var name = GoalName.Text?.Trim() ?? "";
            var endDate = GoalDate.SelectedDate;
            if (endDate == null)
            {
                MessageBox.Show("Wybierz termin wykonania.");
                return;
            }

            var category = (string)GoalCat.SelectionBoxItem;
            var importance = int.Parse((string)GoalImportance.SelectionBoxItem);
            var description = new TextRange(
                GoalDescr.Document.ContentStart,
                GoalDescr.Document.ContentEnd
            ).Text.Trim();
            
            _api.SetToken(UserSession.Token);
            
            
                MessageBoxResult result = MessageBox.Show("Czy chcesz od razu dodać zadania do twojego celu?",
                    "Zawsze możesz dodać je później",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.None);
                
                if (result == MessageBoxResult.Yes)
                {
                    int newGoalId = await _api.AddGoalAsync(name, endDate.Value, category, importance, description);
                    UserSession.Goals = (await _api.GoalsAsync()).getGoals;
                    NavigationService.Navigate(new AddTaskPage(newGoalId));
                }
            
                if (result == MessageBoxResult.No)
                {
                    await _api.AddGoalAsync(name, endDate.Value, category, importance, description);
                    UserSession.Goals = (await _api.GoalsAsync()).getGoals;
                    NavigationService.Navigate(new HomePage());
                }
            }
        }
    }