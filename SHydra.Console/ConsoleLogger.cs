using System;

namespace SHydra
{
    public class ConsoleLogger
    {
        public static void ShowErrorMessage(string message)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[!] ERROR: {message}");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;


        }
        public static void LogMessage(string message, string symbol="*")
        {
            Console.WriteLine($"[{symbol}] {message}");
        }

    }
}
