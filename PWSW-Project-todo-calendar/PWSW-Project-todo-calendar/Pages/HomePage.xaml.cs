using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PWSW_Project_todo_calendar.Config;
using PWSW_Project_todo_calendar.Pages.UserControl;

namespace PWSW_Project_todo_calendar.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
            
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                DesignerPlaceholderHome.Visibility = Visibility.Visible;
                StatsPieHome.Visibility = Visibility.Collapsed;
                return;
            }
            
            
            StatsPieHome.Series = new ISeries[]
            {
                new PieSeries<double>
                {
                    Name = "Zrobione",
                    Values = new[] { 12.0 },
                    DataLabelsSize = 12,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                },
                new PieSeries<double>
                {
                    Name = "W trakcie", 
                    Values = new[] { 5.0 },  
                    DataLabelsSize = 12, 
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                },
                new PieSeries<double>
                {
                    Name = "Nie zrobione", 
                    Values = new[] { 3.0 }, 
                    DataLabelsSize = 12, 
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                }
            };
        }
        
        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGoals();
        }

        private void GoToStats(object sender, RoutedEventArgs routedEventArgs)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }

        private void LoadGoals()
        {
            Goals.Children.Clear();

            foreach (var goal in UserSession.Goals)
            {
                if (!UserSession.TasksByGoal.TryGetValue(goal.idGoal, out var tasks))
                    tasks = new List<TaskDto>();

                var ctrl = new GoalRowControl(goal);
                Goals.Children.Add(ctrl);
            }
        }
        
    }
}