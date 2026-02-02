using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using Microsoft.Win32;
using PWSW_Project_todo_calendar.Config;

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

        this.Loaded += Page_Loaded;
    }
    
    private (double Done, double NotStarted, double InProgress) CalculateStats(List<GoalDto> goals)
    {
        double countDone = 0;
        double countOverdue = 0;
        double countPlanned = 0;
        
        DateTime today = DateTime.Now.Date;

        foreach (var goal in goals)
        {
            if (goal.isFinished)
            {
                countDone++;
            }
            else
            {
                DateTime end = DateTime.MaxValue;
                bool hasDate = DateTime.TryParse(goal.endDate, out end);

                if (hasDate && today > end.Date)
                {
                    countOverdue++;
                }
                else
                {
                    countPlanned++; 
                }
            }
        }
        return (countDone, countOverdue, countPlanned);
    }
    
    
    private void UpdateLeftChart()
    {
        try
        {
            if (MonthCalendar == null) return;

            List<GoalDto> allGoals = UserSession.Goals;
            List<GoalDto> monthGoals = new List<GoalDto>();
            
            int targetMonth = MonthCalendar.DisplayDate.Month;
            int targetYear = MonthCalendar.DisplayDate.Year;

            foreach (var goal in allGoals)
            {
                if (DateTime.TryParse(goal.endDate, out DateTime d))
                {
                    if (d.Month == targetMonth && d.Year == targetYear)
                    {
                        monthGoals.Add(goal);
                    }
                }
                else
                {
                }
            }

            var stats = CalculateStats(monthGoals);

            StatsPieSmall.Series = new ISeries[]
            {
                new PieSeries<double> { Name = "Zrobione", Values = new[] { stats.Done }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(33, 150, 243)) },
                new PieSeries<double> { Name = "Po terminie", Values = new[] { stats.NotStarted }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(244, 67, 54)) },
                new PieSeries<double> { Name = "W czasie", Values = new[] { stats.InProgress }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(76, 175, 80)) }
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Błąd lewego wykresu: " + ex.Message);
        }
    }

    private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
    {
        UpdateLeftChart();
    }
    private void UpdateRightChart()
    {
        try
        {
            if (PickerFrom == null || PickerTo == null) return;

            List<GoalDto> allGoals = UserSession.Goals;
            
            DateTime fromDate = PickerFrom.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = PickerTo.SelectedDate ?? DateTime.MaxValue;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            List<GoalDto> rangeGoals = new List<GoalDto>();

            foreach (var goal in allGoals)
            {
                if (DateTime.TryParse(goal.endDate, out DateTime d))
                {
                    if (d >= fromDate && d <= toDate)
                    {
                        rangeGoals.Add(goal);
                    }
                }
                else
                {
                    rangeGoals.Add(goal); 
                }
            }
            
            var stats = CalculateStats(rangeGoals);

            StatsPie.Series = new ISeries[]
            {
                new PieSeries<double> { Name = "Zrobione", Values = new[] { stats.Done }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(33, 150, 243)) },
                new PieSeries<double> { Name = "Po terminie", Values = new[] { stats.NotStarted }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(244, 67, 54)) },
                new PieSeries<double> { Name = "W czasie", Values = new[] { stats.InProgress }, Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SkiaSharp.SKColor(76, 175, 80)) }
            };
            
             StatsPieSmall.Series = new ISeries[]
            {
                new PieSeries<double> { Name = "Zrobione", Values = new[] { stats.Done } },
                new PieSeries<double> { Name = "Po terminie", Values = new[] { stats.NotStarted } },
                new PieSeries<double> { Name = "W czasie", Values = new[] { stats.InProgress } }
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Błąd prawego wykresu: " + ex.Message);
        }
    }
    
    private void Picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateRightChart();
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (PickerFrom.SelectedDate == null)
            PickerFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        if (PickerTo.SelectedDate == null)
            PickerTo.SelectedDate = DateTime.Now.AddMonths(1);
        
        UpdateLeftChart();
        UpdateRightChart();
    }
    
    private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn != null) btn.IsEnabled = false;

        try
        {
            List<GoalDto> allGoals = UserSession.Goals;
            DateTime fromDate = PickerFrom.SelectedDate ?? DateTime.MinValue;
            DateTime toDate = PickerTo.SelectedDate ?? DateTime.MaxValue;
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            List<GoalDto> filteredGoals = new List<GoalDto>();
            foreach (var goal in allGoals)
            {
                if (DateTime.TryParse(goal.endDate, out DateTime d))
                {
                    if (d >= fromDate && d <= toDate) filteredGoals.Add(goal);
                }
                else filteredGoals.Add(goal);
            }
            
            var stats = CalculateStats(filteredGoals);
            double total = filteredGoals.Count;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Plik PDF (*.pdf)|*.pdf";
            saveFileDialog.FileName = $"Raport_Celow_{DateTime.Now:yyyy-MM-dd}.pdf";

            if (saveFileDialog.ShowDialog() == true)
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Raport Celów";
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont fontHeader = new XFont("Arial", 20);
                XFont fontNormal = new XFont("Arial", 12);
                XFont fontBold = new XFont("Arial", 12);

                gfx.DrawString("Raport Realizacji Celów", fontHeader, XBrushes.DarkBlue, new XPoint(40, 40));
                
                string rangeInfo = $"Zakres: {PickerFrom.SelectedDate:dd.MM.yyyy} - {PickerTo.SelectedDate:dd.MM.yyyy}";
                gfx.DrawString(rangeInfo, fontBold, XBrushes.Black, new XPoint(40, 70));
                
                gfx.DrawString($"Wygenerowano: {DateTime.Now:g}", fontNormal, XBrushes.Gray, new XPoint(40, 90));
                gfx.DrawLine(XPens.Gray, 40, 100, page.Width - 40, 100);

                int yPoint = 130;
                int xCol1 = 40;
                int xCol2 = 280;

                gfx.DrawString("Status celu", fontBold, XBrushes.Black, new XPoint(xCol1, yPoint));
                gfx.DrawString("Liczba", fontBold, XBrushes.Black, new XPoint(xCol2, yPoint));
                yPoint += 25;

                gfx.DrawString("Zrobione (Ukończone)", fontNormal, XBrushes.Blue, new XPoint(xCol1, yPoint));
                gfx.DrawString(stats.Done.ToString(), fontNormal, XBrushes.Black, new XPoint(xCol2, yPoint));
                yPoint += 20;

                gfx.DrawString("Po terminie (Zaległe)", fontNormal, XBrushes.Red, new XPoint(xCol1, yPoint));
                gfx.DrawString(stats.NotStarted.ToString(), fontNormal, XBrushes.Black, new XPoint(xCol2, yPoint));
                yPoint += 20;

                gfx.DrawString("W terminie (Planowane)", fontNormal, XBrushes.Green, new XPoint(xCol1, yPoint));
                gfx.DrawString(stats.InProgress.ToString(), fontNormal, XBrushes.Black, new XPoint(xCol2, yPoint));
                yPoint += 30;

                gfx.DrawLine(XPens.Gray, 40, yPoint - 10, page.Width - 40, yPoint - 10);
                gfx.DrawString($"Łącznie celów w tym okresie: {total}", fontBold, XBrushes.Black, new XPoint(xCol1, yPoint));
                
                document.Save(saveFileDialog.FileName);

                var p = new Process();
                p.StartInfo = new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true };
                p.Start();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd: {ex.Message}", "Ups!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            if (btn != null) btn.IsEnabled = true;
        }
    }
}