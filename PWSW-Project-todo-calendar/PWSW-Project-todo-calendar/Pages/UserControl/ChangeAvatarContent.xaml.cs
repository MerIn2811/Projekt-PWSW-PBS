using System.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PWSW_Project_todo_calendar.Pages.UserControl;

public partial class ChangeAvatarContent : System.Windows.Controls.UserControl
{
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
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad; // ważne: zwalnia plik po wczytaniu
            bmp.UriSource = new Uri(dlg.FileName);
            bmp.EndInit();
            bmp.Freeze();

            AvatarBrush.ImageSource = bmp; // podmiana avatara
        }
    }
}