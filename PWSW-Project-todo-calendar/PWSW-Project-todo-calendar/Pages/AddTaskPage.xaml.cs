using System.Windows;
using System.Windows.Controls;

namespace PWSW_Project_todo_calendar.Pages;

public partial class AddTaskPage : Page
{
    public AddTaskPage()
    {
        InitializeComponent();
    }
    
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(TextBox1.Text))
        {
            ComboBox1.Items.Add(TextBox1.Text);
            TextBox1.Clear();
        }
        else
        {
            MessageBox.Show("Wpisz tekst do dodania.");
        }
    }
}