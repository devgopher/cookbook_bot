namespace CookBookBot.Settings;

public class CookBookSettings
{
    public static string Section => "CookBookSettings";
    public required string DbConnection { get; set; }
}