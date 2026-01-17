using System.Windows;
using System.Windows.Controls;
using PWSW_Project_todo_calendar.Pages.UserControl;

namespace PWSW_Project_todo_calendar.Pages;

public partial class AddTaskPage : Page
{
    public AddTaskPage()
    {
        InitializeComponent();
        AddNewTaskControl();
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

    private void Finish_Click(object sender, RoutedEventArgs e)
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