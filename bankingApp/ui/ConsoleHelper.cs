namespace bankingApp.ui;

public static class ConsoleHelper
{
    public static void Clear() => Console.Clear();

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(intercept: true);
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;

            Console.WriteLine("Please enter a valid number.");
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            int value = ReadInt(prompt);
            if (value > 0)
                return value;

            Console.WriteLine("Please enter a number greater than zero.");
        }
    }

    public static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                return value;

            Console.WriteLine("Please enter an amount greater than zero.");
        }
    }

    public static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            Console.WriteLine("This field is required.");
        }
    }

    public static bool Confirm(string prompt)
    {
        Console.Write($"{prompt} (y/n): ");
        string? response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    // Kept from before -- masked input for passwords specifically.
    public static string ReadPassword(string prompt = "Password: ")
    {
        Console.Write(prompt);
        var password = new System.Text.StringBuilder();
        ConsoleKeyInfo keyInfo;

        while (true)
        {
            keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                password.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
        }

        return password.ToString();
    }
}