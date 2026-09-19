using Microsoft.Extensions.DependencyInjection;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.users;
using bankingApp.services.admin;

namespace bankingApp.ui;

public class AdminMenu
{
    private readonly IAdminService adminService;
    private readonly Admin admin;

    public AdminMenu(IServiceProvider scopedProvider, Admin loggedInAdmin)
    {
        adminService = scopedProvider.GetRequiredService<IAdminService>();
        admin = loggedInAdmin;
    }

    public void Run()
    {
        bool isWorking = true;
        while (isWorking)
        {
            Console.WriteLine("===== ADMIN VIEW =====\n");
            Console.WriteLine($"Welcome Back, {admin.Username}!");
            Console.WriteLine("1. Create New Customer");
            Console.WriteLine("2. Create Bank Account For Customer");
            Console.WriteLine("3. Delete Customer");
            Console.WriteLine("4. Delete Bank Account");
            Console.WriteLine("5. Edit Customer Username");
            Console.WriteLine("6. Display Summary");
            Console.WriteLine("7. Reset Customer Password");
            Console.WriteLine("8. Approve/Reject Cheque Book Requests");
            Console.WriteLine("9. Exit");
            Console.Write("Please Enter Your Choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid Choice. Please Try Again.");
                continue;
            }

            switch (choice)
            {
                case 1: HandleCreateCustomer(); break;
                case 2: HandleCreateBankAccount(); break;
                case 3: HandleDeleteCustomer(); break;
                case 4: HandleDeleteBankAccount(); break;
                case 5: HandleEditUsername(); break;
                case 6: ShowSummary(); break;
                case 7: HandleResetPassword(); break;
                case 8: HandleChequeBookApprovals(); break;
                case 9:
                    Console.WriteLine("Returning To Login...");
                    isWorking = false;
                    break;
                default: Console.WriteLine("Invalid Choice. Please Try Again."); break;
            }
        }
    }

    private void HandleCreateCustomer()
    {
        Console.Write("Enter Username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Enter Password: ");
        string password = ConsoleHelper.ReadPassword();

        var result = adminService.CreateCustomer(username, password);
        Console.WriteLine(result.Success ? "Customer created successfully." : result.Error);
    }

    private void HandleCreateBankAccount()
    {
        Console.Write("Enter Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid customer ID.");
            return;
        }

        // Enum-driven menu -- only ever shows the account types that actually exist,
        // and there's no way to type something that isn't one of them.
        var accountTypes = Enum.GetValues<AccountType>();
        Console.WriteLine("\nSelect Account Type:");
        for (int i = 0; i < accountTypes.Length; i++)
            Console.WriteLine($"{i + 1}. {accountTypes[i]}");

        Console.Write("Choice: ");
        if (!int.TryParse(Console.ReadLine(), out int typeChoice) ||
            typeChoice < 1 || typeChoice > accountTypes.Length)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        AccountType selectedType = accountTypes[typeChoice - 1];

        Console.Write("Enter Initial Balance: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        var result = adminService.CreateBankAccount(customerId, selectedType, balance);
        Console.WriteLine(result.Success ? $"{selectedType} account created." : result.Error);
    }

    private void HandleDeleteCustomer()
    {
        Console.Write("Enter Customer ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid customer ID.");
            return;
        }

        var result = adminService.DeleteCustomer(customerId);
        Console.WriteLine(result.Success ? "Customer deleted." : result.Error);
    }

    private void HandleDeleteBankAccount()
    {
        Console.Write("Enter Account ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        {
            Console.WriteLine("Invalid account ID.");
            return;
        }

        var result = adminService.DeleteBankAccount(accountId);
        Console.WriteLine(result.Success ? "Account deleted." : result.Error);
    }

    private void HandleEditUsername()
    {
        Console.Write("Enter Customer ID to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid customer ID.");
            return;
        }

        Console.Write("Enter new username: ");
        string newUsername = Console.ReadLine() ?? "";

        var result = adminService.EditCustomerUsername(customerId, newUsername);
        Console.WriteLine(result.Success ? "Username updated." : result.Error);
    }

    private void ShowSummary()
    {
        var summary = adminService.GetSummary();
        Console.WriteLine($"Total Customers: {summary.CustomerCount}");
        Console.WriteLine($"Total Bank Accounts: {summary.AccountCount}");
        Console.WriteLine($"Total Balance Across All Accounts: ${summary.TotalBalance:F2}");
        Console.WriteLine($"Total Transactions Recorded: {summary.TransactionCount}");
    }

    private void HandleResetPassword()
    {
        Console.Write("Enter Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid customer ID.");
            return;
        }

        Console.Write("Enter new password: ");
        string newPassword = ConsoleHelper.ReadPassword();

        var result = adminService.ResetCustomerPassword(customerId, newPassword);
        Console.WriteLine(result.Success ? "Password reset successfully." : result.Error);
    }

    private void HandleChequeBookApprovals()
    {
        var pending = adminService.GetPendingChequeBookRequests();
        if (pending.Count == 0)
        {
            Console.WriteLine("No pending requests.");
            return;
        }

        foreach (var request in pending)
            Console.WriteLine($"Request #{request.RequestId} | Account: {request.AccountId} | Requested: {request.RequestDate:g}");

        Console.Write("\nEnter Request ID to approve/reject (or 0 to go back): ");
        if (!int.TryParse(Console.ReadLine(), out int requestId) || requestId == 0)
            return;

        Console.Write("Approve or Reject? (A/R): ");
        string? action = Console.ReadLine()?.Trim().ToUpper();

        var result = action switch
        {
            "A" => adminService.ApproveChequeBookRequest(requestId),
            "R" => adminService.RejectChequeBookRequest(requestId),
            _ => banking_common_Result_Fail()
        };

        Console.WriteLine(result.Success ? "Request processed." : result.Error);

        static bankingApp.common.Result banking_common_Result_Fail() =>
            bankingApp.common.Result.Fail("Invalid action.");
    }
}