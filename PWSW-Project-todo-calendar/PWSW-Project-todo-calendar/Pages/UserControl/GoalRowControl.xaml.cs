using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Newtonsoft.Json;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class GoalRowControl : System.Windows.Controls.UserControl
{
    private readonly int _goalId;
    private readonly ApiClient _api = new ApiClient("https://lotekweronika.pl/api/");
    
    public GoalRowControl(GoalDto goal)
    {
        InitializeComponent();
        _goalId = goal.idGoal;
        Loaded += GoalPage_Loaded;
        if (goal.isFinished == true) IsFinishedBox.IsChecked = true;
        else IsFinishedBox.IsChecked = false;

        IsFinishedBox.Content = goal.name;
        StartDateText.Text = FormatDate(goal.startDate);
        EndDateText.Text   = FormatDate(goal.endDate);
        Category.Text = goal.category;

        switch (goal.importance)
        {
            case 0:
                Colour.Background = new SolidColorBrush(Colors .Pink);
                break;
            case 1:
                Colour.Background = new SolidColorBrush(Colors.DarkSalmon);
                break;
            case 2:
                Colour.Background = new SolidColorBrush(Colors.OrangeRed);
                break;
            default:
                Colour.Background = new SolidColorBrush(Colors.White);
                break;
        }
        
    }

    private void GoalPage_Loaded(object sender, RoutedEventArgs e)
    {
        LoadTasks();
        LoadProgress();
    }

    private void LoadProgress()
    {
        int maxProgress = UserSession.TasksByGoal[_goalId].Count;
        if (maxProgress == 0)
        {
            if (IsFinishedBox.IsChecked == true) TasksFinishedBar.Value = 100;
            else TasksFinishedBar.Value = 0;
            
        }
        else
        {
            int currentProgress = 0;
            foreach (var task in UserSession.TasksByGoal[_goalId])
            {
                if (task.isFinished == true)
                {
                    currentProgress += 1;
                }
            }

            if (currentProgress < maxProgress)
            {
                IsFinishedBox.IsChecked = false;
            }

            double finalValue = (double)currentProgress / maxProgress * 100.0;
            if (finalValue >= 100.0)
            {
                IsFinishedBox.IsChecked = true;
            }
            TasksFinishedBar.Value = finalValue;
        }
    }

    private void LoadTasks()
    {
        TaskPanel.Children.Clear();

        if (UserSession.TasksByGoal[_goalId].Count == 0)
        {
            ToggleTask.IsEnabled = false;
            ToggleTask.Visibility = Visibility.Hidden;
        }

        foreach (var task in UserSession.TasksByGoal[_goalId])
        {
            var ctrl = new TaskRowControl(task, _goalId, RefreshThisGoalAsync);
            TaskPanel.Children.Add(ctrl);
        }
    }
    
    private void ToggleTask_Checked(object sender, RoutedEventArgs e)
    {  
        TasksPanel.Visibility = Visibility.Visible;
        ToggleTask.Content = "▲";
    }

    private void ToggleTask_Unchecked(object sender, RoutedEventArgs e)
    {
        TasksPanel.Visibility = Visibility.Collapsed;
        ToggleTask.Content = "▼";
    }

    private void goGoalControlClick(object sender, RoutedEventArgs e)
    {
        var nav = NavigationService.GetNavigationService(this);
        nav?.Navigate(new SettingsEditPage(SettingsEditMode.Goal));
    }
    
    private string FormatDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        if (DateTime.TryParse(value, out var dt))
            return dt.ToString("dd.MM.yyyy HH:mm");

        return value;
    }

    private async void Checked(object sender, RoutedEventArgs e)
    {

        try
        {
            var isFinished = IsFinishedBox.IsChecked == true ? 1 : 0;
            _api.SetToken(UserSession.Token);

            await _api.PatchGoalAsyncIsFinished(_goalId, new
            {
                isFinished = isFinished
            });

            UserSession.Goals = (await _api.GoalsAsync()).getGoals;
            LoadProgress();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się zaktualizować goal:\n" + ex.Message);
        }
    }
    
    private async Task RefreshThisGoalAsync()
    {
        LoadProgress();
        
        _api.SetToken(UserSession.Token);
        var goalsRes = await _api.GoalsAsync();
        var fresh = goalsRes.getGoals.FirstOrDefault(g => g.idGoal == _goalId);
        if (fresh != null)
        {
            var idx = UserSession.Goals.FindIndex(g => g.idGoal == _goalId);
            if (idx >= 0) UserSession.Goals[idx] = fresh;
        }
    }
}