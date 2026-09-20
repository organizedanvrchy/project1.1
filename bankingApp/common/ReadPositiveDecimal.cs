using bankingApp.common;

public static class ReadPositiveDecimal
{
    public static decimal Read(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (!decimal.TryParse(input, out decimal value))
            {
                Console.WriteLine("Please enter a valid amount.");
                continue;
            }

            if (value <= 0)
            {
                Console.WriteLine("Please enter an amount greater than zero.");
                continue;
            }

            if (value > TransactionLimits.MaxAmount)
            {
                Console.WriteLine($"Amount is too large. Maximum allowed is {TransactionLimits.MaxAmount:C}.");
                continue;
            }

            return value;
        }
    }
}