using Microsoft.Extensions.DependencyInjection;
using bankingApp.data.entities.users;
using bankingApp.services.auth;

namespace bankingApp.ui;

public class MainMenu
{
    private readonly IServiceProvider provider;

    public MainMenu(IServiceProvider serviceProvider)
    {
        provider = serviceProvider;
    }

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            ConsoleHelper.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine("         WELCOME TO THE BANK");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Exit");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1:
                    HandleLogin();
                    break;

                case 2:
                    isRunning = false;
                    Console.WriteLine();
                    Console.WriteLine("Thank you for banking with us!");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private void HandleLogin()
    {
        using var scope = provider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("                LOGIN");
        Console.WriteLine("========================================");
        Console.WriteLine();

        string username = ConsoleHelper.ReadRequiredString("Username: ");
        string password = ConsoleHelper.ReadPassword();

        var result = authService.Login(username, password);
        if (!result.Success)
        {
            Console.WriteLine();
            Console.WriteLine(result.Error);
            ConsoleHelper.Pause();
            return;
        }

        switch (result.Value)
        {
            case Customer customer:
                new CustomerMenu(scope.ServiceProvider, customer).Run();
                break;

            case Admin admin:
                new AdminMenu(scope.ServiceProvider, admin).Run();
                break;

            default:
                Console.WriteLine("Unrecognized account type.");
                ConsoleHelper.Pause();
                break;
        }
    }
}