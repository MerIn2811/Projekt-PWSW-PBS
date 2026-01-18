using System.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using PWSW_Project_todo_calendar.Config;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class ChangeAvatarContent : System.Windows.Controls.UserControl
{
    
    public string? SelectedAvatarPath { get; private set; }
    
    public ChangeAvatarContent()
    {
        InitializeComponent();
    }
    
    private void ChooseAvatar_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Wybierz avatar",
            Filter = "Obrazy (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Wszystkie pliki (*.*)|*.*"
        };

        if (dlg.ShowDialog() == true)
        {
            
            SelectedAvatarPath = dlg.FileName;
            
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad; 
            bmp.UriSource = new Uri(dlg.FileName);
            bmp.EndInit();
            bmp.Freeze();

            AvatarBrush.ImageSource = bmp;
        }
    }
}