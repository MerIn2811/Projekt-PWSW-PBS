using System.Windows;
using System.Windows.Controls;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Pages.UserControl;

namespace PWSW_Project_todo_calendar.Pages;

public partial class AddTaskPage : Page
{
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    private readonly int _goalId;
    
    public AddTaskPage(int goalId)
    {
        InitializeComponent();
        AddNewTaskControl();
        _goalId = goalId;
    }

    private void AddTask_Click(object sender, RoutedEventArgs e)
    {
        AddNewTaskControl();
    }

    private void AddNewTaskControl()
    {
        var ctrl = new TaskAdd();
        ctrl.RemoveRequested += (_, __) =>
        {
            TasksContainer.Children.Remove(ctrl);
            UpdateDeleteButtons();
        };
        

        TasksContainer.Children.Add(ctrl);
        UpdateDeleteButtons();
    }

    private async void Finish_Click(object sender, RoutedEventArgs e)
    {
        var tasks = TasksContainer.Children
            .OfType<TaskAdd>()
            .Select(c => c.GetData())
            .ToList();
        
        if (tasks.Any(t => string.IsNullOrWhiteSpace(t.Name)))
        {
            MessageBox.Show("Uzupełnij nazwę we wszystkich zadaniach.");
            return;
        }
        
        //Dodanie tasków po kolei do bazy
        foreach (var task in tasks)
        {
            if (tasks.Any(t => t.DueDate == null))
            {
                MessageBox.Show("Uzupełnij termin we wszystkich zadaniach.");
                return;
            }
            var name = task.Name;
            var endDate = task.DueDate!.Value;
            var description = task.Description;
            int importance = int.Parse(task.Priority);
            
            _api.SetToken(UserSession.Token);
            
            int newTaskId = await _api.AddTaskAsync(_goalId,name, endDate, importance, description);
            
            UserSession.TasksByGoal = new Dictionary<int, List<TaskDto>>();
            foreach (var g in UserSession.Goals)
            {
                var tasksRes = await _api.TasksAsync(g.idGoal);
                UserSession.TasksByGoal[g.idGoal] = tasksRes.tasks;
            }
            
        }
        
        MessageBoxResult result = MessageBox.Show($"Zebrano {tasks.Count} zadań.\nPierwsze ID: {tasks[0].Id}", null, MessageBoxButton.OK, MessageBoxImage.Information);

        if (result == MessageBoxResult.OK)
        {
            NavigationService.Navigate(new HomePage());
        }
    }
    
    private void UpdateDeleteButtons()
    {
        bool showDelete = TasksContainer.Children.Count > 1;

        foreach (var task in TasksContainer.Children.OfType<TaskAdd>())
        {
            task.SetDeleteVisible(showDelete);
        }
    }
    
}