using Microsoft.Extensions.DependencyInjection;
using bankingApp.common;
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
            ConsoleHelper.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine($"     ADMIN VIEW -- {admin.Username.ToUpper()}");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("1. Create New Customer");
            Console.WriteLine("2. Create New Administrator");
            Console.WriteLine("3. Create Bank Account For Customer");
            Console.WriteLine("4. Delete Customer");
            Console.WriteLine("5. Delete Bank Account");
            Console.WriteLine("6. Edit Customer Username");
            Console.WriteLine("7. Display Summary");
            Console.WriteLine("8. Reset Customer Password");
            Console.WriteLine("9. Approve/Reject Cheque Book Requests");
            Console.WriteLine("10. Exit");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1: HandleCreateCustomer(); break;
                case 2: HandleCreateAdmin(); break;
                case 3: HandleCreateBankAccount(); break;
                case 4: HandleDeleteCustomer(); break;
                case 5: HandleDeleteBankAccount(); break;
                case 6: HandleEditUsername(); break;
                case 7: ShowSummary(); break;
                case 8: HandleResetPassword(); break;
                case 9: HandleChequeBookApprovals(); break;
                case 10:
                    isWorking = false;
                    Console.WriteLine();
                    Console.WriteLine("Returning to login...");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private void HandleCreateCustomer()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("            CREATE CUSTOMER");
        Console.WriteLine("========================================");
        Console.WriteLine();

        string username = ConsoleHelper.ReadRequiredString("Username: ");
        string password = ConsoleHelper.ReadPassword("Password: ");

        var result = adminService.CreateCustomer(username, password);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Customer created successfully." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleCreateAdmin()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("            CREATE ADMIN");
        Console.WriteLine("========================================");
        Console.WriteLine();

        string username = ConsoleHelper.ReadRequiredString("Username: ");
        string password = ConsoleHelper.ReadPassword("Password: ");

        var result = adminService.CreateAdmin(username, password);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Admin created successfully." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleCreateBankAccount()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("           CREATE BANK ACCOUNT");
        Console.WriteLine("========================================");
        Console.WriteLine();

        int customerId = ConsoleHelper.ReadPositiveInt("Customer ID: ");

        var accountTypes = Enum.GetValues<AccountType>();
        Console.WriteLine();
        Console.WriteLine("Account Type");
        Console.WriteLine("----------------------------------------");
        for (int i = 0; i < accountTypes.Length; i++)
            Console.WriteLine($"{i + 1}. {accountTypes[i]}");
        Console.WriteLine();

        int typeChoice = ConsoleHelper.ReadInt("Select account type: ");
        if (typeChoice < 1 || typeChoice > accountTypes.Length)
        {
            Console.WriteLine();
            Console.WriteLine("Invalid selection.");
            ConsoleHelper.Pause();
            return;
        }

        AccountType selectedType = accountTypes[typeChoice - 1];
        decimal balance = ConsoleHelper.ReadPositiveDecimal("Initial balance: ");

        var result = adminService.CreateBankAccount(customerId, selectedType, balance);

        Console.WriteLine();
        Console.WriteLine(result.Success ? $"{selectedType} account created." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleDeleteCustomer()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("            DELETE CUSTOMER");
        Console.WriteLine("========================================");
        Console.WriteLine();

        int customerId = ConsoleHelper.ReadPositiveInt("Customer ID: ");

        if (!ConsoleHelper.Confirm($"Are you sure you want to delete customer {customerId}?"))
        {
            Console.WriteLine("Cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        var result = adminService.DeleteCustomer(customerId);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Customer deleted." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleDeleteBankAccount()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("          DELETE BANK ACCOUNT");
        Console.WriteLine("========================================");
        Console.WriteLine();

        int accountId = ConsoleHelper.ReadPositiveInt("Account ID: ");

        if (!ConsoleHelper.Confirm($"Are you sure you want to delete account {accountId}?"))
        {
            Console.WriteLine("Cancelled.");
            ConsoleHelper.Pause();
            return;
        }

        var result = adminService.DeleteBankAccount(accountId);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Account deleted." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleEditUsername()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("          EDIT CUSTOMER USERNAME");
        Console.WriteLine("========================================");
        Console.WriteLine();

        int customerId = ConsoleHelper.ReadPositiveInt("Customer ID: ");
        string newUsername = ConsoleHelper.ReadRequiredString("New username: ");

        var result = adminService.EditCustomerUsername(customerId, newUsername);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Username updated." : result.Error);
        ConsoleHelper.Pause();
    }

    private void ShowSummary()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("                SUMMARY");
        Console.WriteLine("========================================");
        Console.WriteLine();

        AdminSummary summary = adminService.GetSummary();

        if (summary.Customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine($"{"ID",-5} {"USERNAME",-20} {"ACCOUNTS",-10} {"BALANCE",12} {"LOANS",12} {"NET BALANCE",12}");
        Console.WriteLine("--------------------------------------------------------------------------------------------------");

        foreach (var customer in summary.Customers)
        {
            Console.WriteLine($"{customer.CustomerId,-5} {customer.Username,-20} {customer.AccountCount,-10} {customer.TotalBalance,12:C} {customer.TotalLoanBalance,12:C} {customer.NetBalance,12:C}");
        }

        Console.WriteLine("--------------------------------------------------------------------------------------------------");
        Console.WriteLine($"{"TOTAL ",-5}{summary.Customers.Count + " customers",-20} {summary.TotalAccountCount,-10} {summary.TotalBalance,12:C} {summary.TotalLoanBalance,12:C} {summary.TotalNetBalance,12:C}");
        Console.WriteLine("--------------------------------------------------------------------------------------------------");

        Console.WriteLine();
        Console.WriteLine($"Total Transactions Recorded: {summary.TotalTransactionCount}");

        ConsoleHelper.Pause();
    }

    private void HandleResetPassword()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("          RESET CUSTOMER PASSWORD");
        Console.WriteLine("========================================");
        Console.WriteLine();

        int customerId = ConsoleHelper.ReadPositiveInt("Customer ID: ");
        string newPassword = ConsoleHelper.ReadPassword("New password: ");

        var result = adminService.ResetCustomerPassword(customerId, newPassword);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Password reset successfully." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleChequeBookApprovals()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("        CHEQUE BOOK APPROVALS");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var pending = adminService.GetPendingChequeBookRequests();
        if (pending.Count == 0)
        {
            Console.WriteLine("No pending requests.");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine("ID    ACCOUNT   REQUESTED");
        Console.WriteLine("----------------------------------------");
        foreach (var request in pending)
            Console.WriteLine($"{request.RequestId,-5} {request.AccountId,-9} {request.RequestDate:g}");

        Console.WriteLine();
        int requestId = ConsoleHelper.ReadInt("Request ID to approve/reject (0 to go back): ");
        if (requestId == 0) return;

        string action = ConsoleHelper.ReadRequiredString("Approve or Reject? (A/R): ").ToUpper();

        var result = action switch
        {
            "A" => adminService.ApproveChequeBookRequest(requestId),
            "R" => adminService.RejectChequeBookRequest(requestId),
            _ => Result.Fail("Invalid action.")
        };

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Request processed." : result.Error);
        ConsoleHelper.Pause();
    }
}