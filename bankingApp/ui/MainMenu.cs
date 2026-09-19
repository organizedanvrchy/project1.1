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
            Console.WriteLine("===== Welcome =====");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Exit\n");
            Console.Write("Please Select An Option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid Choice.\n");
                continue;
            }

            switch (choice)
            {
                case 1:
                    HandleLogin();
                    break;
                case 2:
                    isRunning = false;
                    Console.WriteLine("Thank you for using our banking application. See you again!");
                    break;
                default:
                    Console.WriteLine("Invalid Choice.\n");
                    break;
            }
        }
    }

    private void HandleLogin()
    {
        using var scope = provider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        Console.WriteLine("\n===== LOGIN =====\n");
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        string password = ConsoleHelper.ReadPassword();

        var result = authService.Login(username, password);
        if (!result.Success)
        {
            Console.WriteLine($"\n{result.Error}\n");
            return;
        }

        // One login path -- the account type decides where you land, not a separate menu choice.
        switch (result.Value)
        {
            case Customer customer:
                Console.WriteLine($"\nLogin Successful. Welcome {customer.Username}!\n");
                new CustomerMenu(scope.ServiceProvider, customer).Run();
                break;

            case Admin admin:
                Console.WriteLine($"\nLogin Successful. Welcome Admin {admin.Username}!\n");
                new AdminMenu(scope.ServiceProvider, admin).Run();
                break;

            default:
                Console.WriteLine("\nUnrecognized account type.\n");
                break;
        }
    }
}