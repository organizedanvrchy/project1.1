namespace bankingApp.ui;

public static class ConsoleHelper
{
    public static string ReadPassword()
    {
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