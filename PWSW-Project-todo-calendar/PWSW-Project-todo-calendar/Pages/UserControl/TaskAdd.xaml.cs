using System.Windows;
using System.Windows.Controls;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class TaskAdd : System.Windows.Controls.UserControl
{
    
    public Guid TaskId { get; set; } = Guid.NewGuid();
    
    public event EventHandler? RemoveRequested;
    
    public TaskAdd()
    {
        InitializeComponent();
    }

    public TaskDraft GetData()
    {
        var selected = CbPriority.SelectedItem as ComboBoxItem;
        string priority = selected?.Content.ToString() ?? "Normal";

        return new TaskDraft
        {
            Id = TaskId,
            Name = TbName.Text,
            DueDate = DpDueDate.SelectedDate,
            Description = TbDesc.Text,
            Priority = priority
        };
    }
    
    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        RemoveRequested?.Invoke(this, EventArgs.Empty);
    }
    
    public void SetDeleteVisible(bool visible)
    {
        deleteBtn.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
    
}

public class TaskDraft
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "Normal";
}

