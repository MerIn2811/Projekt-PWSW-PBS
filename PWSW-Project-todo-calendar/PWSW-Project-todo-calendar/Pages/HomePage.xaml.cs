using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Pages.UserControl;
using System.Collections.ObjectModel;

namespace PWSW_Project_todo_calendar.Pages
{
    public partial class HomePage : Page
    {
        
        private PieSeries<double> _finishedSeries = null!;
        private PieSeries<double> _inProgressSeries = null!;
        private PieSeries<double> _lateSeries = null!;
        
        public HomePage()
        {
            
            InitializeComponent();
            
            Categorie.ItemsSource = Enum.GetValues(typeof(Categories));
            SetDefaultValues();
            
            Calendar.SelectedDate = DateTime.Today;
            
            
            Loaded += HomePage_Loaded;
            
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DesignerPlaceholderHome.Visibility = Visibility.Visible;
                StatsPieHome.Visibility = Visibility.Collapsed;
                return;
            }
            
            
            _finishedSeries = new PieSeries<double>
            {
                Name = "Zrobione",
                Values = new double[] { 0 },
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            };

            _inProgressSeries = new PieSeries<double>
            {
                Name = "W trakcie",
                Values = new double[] { 0 },
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            };

            _lateSeries = new PieSeries<double>
            {
                Name = "Spóźnione",
                Values = new double[] { 0 },
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            };

            StatsPieHome.Series = new ISeries[]
            {
                _finishedSeries,
                _inProgressSeries,
                _lateSeries
            };
            
        }
        
        private void UpdatePieChart()
        {
            var finished = FinishedTasks();
            var notFinished = NotFinishedTasks();
            var late = LateTasks();
            
            _finishedSeries.Values = new double[] { finished };
            _inProgressSeries.Values = new double[] { notFinished };
            _lateSeries.Values = new double[] { late };
            
            
            
        }

        private void SetDefaultValues()
        {
            DateFrom.SelectedDate = null;
            DateTo.SelectedDate = null;
            
            Categorie.SelectedItem = Categories.Wszystkie;
        }
        
        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGoals();
            LoadGoalsForOneDay(Calendar.SelectedDate);
            UpdatePieChart();

        }

        private void GoToStats(object sender, RoutedEventArgs routedEventArgs)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }

        private void LoadGoals(DateTime? from = null, DateTime? to = null, Categories category = Categories.Wszystkie)
        {
            Goals.Children.Clear();

            DateTime? fromDate = from?.Date;
            DateTime? toInclusive = to?.Date.AddDays(1).AddTicks(-1);

            bool filterByCategory = category != Categories.Wszystkie;
            string selectedCategory = category.ToString();

            foreach (var goal in UserSession.Goals)
            {
                if (filterByCategory)
                {
                    bool matchesSelected =
                        string.Equals(goal.category, selectedCategory, StringComparison.OrdinalIgnoreCase);

                    bool isUnknown = !IsKnownCategory(goal.category);

                    if (category == Categories.Inne)
                    {
                        if (!matchesSelected && !isUnknown)
                            continue;
                    }
                    else
                    {
                        if (!matchesSelected)
                            continue;
                    }
                }
                
                if (!UserSession.TasksByGoal.TryGetValue(goal.idGoal, out var tasks))
                    tasks = new List<TaskDto>();

                List<TaskDto> filteredTasks = tasks;

                if (tasks.Count > 0)
                {
                    filteredTasks = tasks.Where(t =>
                    {
                        var taskDate = TryParseDate(t.endDate);
                        if (taskDate == null) return true;

                        if (fromDate != null && taskDate.Value < fromDate.Value) return false;
                        if (toInclusive != null && taskDate.Value > toInclusive.Value) return false;

                        return true;
                    }).ToList();
                }
                
                bool filterByDate = fromDate != null || toInclusive != null;
                if (filterByDate && tasks.Count > 0 && filteredTasks.Count == 0)
                    continue;

                var ctrl = new GoalRowControl(goal);
                ctrl.GoalStatusChanged += (_, __) =>
                {
                    UpdatePieChart();
                    LoadGoalsForOneDay(Calendar.SelectedDate);
                };
                

                Goals.Children.Add(ctrl);
            }
        }


        private void FilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            DateTime? from = DateFrom.SelectedDate;
            DateTime? to = DateTo.SelectedDate;

            if (from != null && to != null && from.Value.Date > to.Value.Date)
            {
                MessageBox.Show("Data 'Od' nie może być późniejsza niż data 'Do'.");
                return;
            }

            var category = (Categories)Categorie.SelectedItem;

            LoadGoals(from, to, category);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            SetDefaultValues();
            LoadGoals();
        }
        
        private static DateTime? TryParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return DateTime.TryParse(value, out var dt) ? dt : null;
        }
        
        
        private static bool IsKnownCategory(string? category)
        {
            if (string.IsNullOrWhiteSpace(category)) return false;
            
            return Enum.TryParse<Categories>(category, ignoreCase: true, out var parsed)
                   && parsed != Categories.Wszystkie;
        }

        private void Calendar_OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selected = Calendar.SelectedDate;
            LoadGoalsForOneDay(selected);
        }


        private void LoadGoalsForOneDay(DateTime? day)
        {
            GoalsByDay.Children.Clear();
            if (day == null) return;

            var selected = day.Value.Date;

            foreach (var goal in UserSession.Goals)
            {
                if (!UserSession.TasksByGoal.TryGetValue(goal.idGoal, out var tasks))
                    tasks = new List<TaskDto>();

                
                bool hasTaskThatDay = tasks.Any(t =>
                {
                    var dt = TryParseDate(t.endDate);
                    return dt != null && dt.Value.Date == selected;
                });

              
                bool hasGoalEndDateThatDay = false;

                if (goal.endDate != null)
                {
                    var goalEnd = TryParseDate(goal.endDate);
                    hasGoalEndDateThatDay = goalEnd != null && goalEnd.Value.Date == selected;
                }

                
                if (!hasTaskThatDay && !hasGoalEndDateThatDay)
                    continue;

                var ctrl = new GoalRowControl(goal);
                ctrl.GoalStatusChanged += (_, __) =>
                {
                    UpdatePieChart();
                    LoadGoals();
                };
                GoalsByDay.Children.Add(ctrl);
            }
        }

        private double FinishedTasks()
        {
            double total = 0;
            foreach (var goal in UserSession.Goals)
            {
                if (goal.isFinished) total++;
            }
            
            return total;
        }

        private double NotFinishedTasks()
        {
            double total = 0;
            var today = DateTime.Today;

            foreach (var goal in UserSession.Goals)
            {
                if (goal.isFinished)
                    continue;

                var end = TryParseDate(goal.endDate);
                
                if (end == null || end.Value.Date >= today)
                    total++;
            }

            return total;
        }
        
        private double LateTasks()
        {
            double total = 0;
            var today = DateTime.Today;

            foreach (var goal in UserSession.Goals)
            {
                if (goal.isFinished)
                    continue;

                var end = TryParseDate(goal.endDate);
                
                if (end != null && end.Value.Date < today)
                    total++;
            }

            return total;
        }

    }
    
    
}