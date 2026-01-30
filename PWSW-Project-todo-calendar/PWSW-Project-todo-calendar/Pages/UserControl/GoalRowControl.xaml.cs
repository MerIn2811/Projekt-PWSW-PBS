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
        if (!UserSession.TasksByGoal.TryGetValue(_goalId, out var tasks))
        {
            TasksFinishedBar.Value = IsFinishedBox.IsChecked == true ? 100 : 0;
            return;
        }

        int maxProgress = tasks.Count;
        if (maxProgress == 0)
        {
            TasksFinishedBar.Value = IsFinishedBox.IsChecked == true ? 100 : 0;
            return;
        }

        int currentProgress = tasks.Count(t => t.isFinished == true);

        if (currentProgress < maxProgress)
            IsFinishedBox.IsChecked = false;

        double finalValue = (double)currentProgress / maxProgress * 100.0;

        if (finalValue >= 100.0)
            IsFinishedBox.IsChecked = true;

        TasksFinishedBar.Value = finalValue;
    }


    private void LoadTasks()
    {
        TaskPanel.Children.Clear();

        if (!UserSession.TasksByGoal.TryGetValue(_goalId, out var tasks))
        {
            ToggleTask.IsEnabled = false;
            ToggleTask.Visibility = Visibility.Hidden;
            return;
        }

        if (tasks.Count == 0)
        {
            ToggleTask.IsEnabled = false;
            ToggleTask.Visibility = Visibility.Hidden;
            return;
        }

        foreach (var task in tasks)
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
        nav?.Navigate(new SettingsEditPage(SettingsEditMode.Goal, _goalId));
    }
    
    private string FormatDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        if (DateTime.TryParse(value, out var dt))
            return dt.ToString("dd.MM.yyyy");

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