using System.IO;

namespace PWSW_Project_todo_calendar.Config;

public class TokenStorage
{
    private static string DirPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PWSW_Project_todo_calendar");

    private static string FilePath => Path.Combine(DirPath, "token.txt");

    public static void Save(string token)
    {
        Directory.CreateDirectory(DirPath);
        File.WriteAllText(FilePath, token ?? string.Empty);
    }

    public static string Load()
    {
        if (!File.Exists(FilePath)) return string.Empty;
        return (File.ReadAllText(FilePath) ?? string.Empty).Trim();
    }

    public static void Clear()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}