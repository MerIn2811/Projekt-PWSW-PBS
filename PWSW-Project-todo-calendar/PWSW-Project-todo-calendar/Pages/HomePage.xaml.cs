using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace PWSW_Project_todo_calendar.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            
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

        private void GoToStats(object sender, RoutedEventArgs routedEventArgs)
        {
            NavigationService?.Navigate(new StatisticsPage());
        }
        
    }
}