namespace SleepyDice.Utilites;

public class ChatUtilities{
    public static string Color(string hexColor, string text) => $"<color={hexColor}>{text}</color>"; 
}