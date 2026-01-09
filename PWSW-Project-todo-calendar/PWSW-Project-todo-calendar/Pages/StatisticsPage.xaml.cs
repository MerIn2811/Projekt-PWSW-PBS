using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace PWSW_Project_todo_calendar.Pages;

public partial class StatisticsPage : Page
{
    public StatisticsPage()
    {
        InitializeComponent();
        
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            DesignerPlaceholder.Visibility = Visibility.Visible;
            StatsPie.Visibility = Visibility.Collapsed;
            return;
        }
        
        StatsPie.Series = new ISeries[]
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
        
        StatsPieSmall.Series = new ISeries[]
        {
            new PieSeries<double>
            {
                Name = "Zrobione",
                Values = new[] { 40.0 },
                DataLabelsSize = 12,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            },
            new PieSeries<double>
            {
                Name = "W trakcie", 
                Values = new[] { 56.0 },  
                DataLabelsSize = 12, 
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            },
            new PieSeries<double>
            {
                Name = "Nie zrobione", 
                Values = new[] { 78.0 }, 
                DataLabelsSize = 12, 
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            }
        };
    }
}