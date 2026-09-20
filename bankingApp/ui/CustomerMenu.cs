using Microsoft.Extensions.DependencyInjection;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.users;
using bankingApp.services.customer;

namespace bankingApp.ui;

public class CustomerMenu
{
    private readonly ICustomerService customerService;
    private readonly Customer customer;

    public CustomerMenu(IServiceProvider scopedProvider, Customer loggedInCustomer)
    {
        customerService = scopedProvider.GetRequiredService<ICustomerService>();
        customer = loggedInCustomer;
    }

    public void Run()
    {
        bool isWorking = true;
        while (isWorking)
        {
            ConsoleHelper.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine($"    WELCOME, {customer.Username.ToUpper()}");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("1. View My Accounts");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Recent Transactions");
            Console.WriteLine("6. Request Cheque Book");
            Console.WriteLine("7. Change Password");
            Console.WriteLine("8. Exit");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1: ShowAccounts(); break;
                case 2: HandleWithdraw(); break;
                case 3: HandleDeposit(); break;
                case 4: HandleTransfer(); break;
                case 5: HandleRecentTransactions(); break;
                case 6: HandleChequeBookRequest(); break;
                case 7: HandleChangePassword(); break;
                case 8: isWorking = false; break;
                default:
                    Console.WriteLine("Invalid option.");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private static string TypeName(BankAccount account) => account switch
    {
        CheckingAccount => "Checking",
        SavingsAccount => "Savings",
        LoanAccount => "Loan",
        _ => "Unknown"
    };

    private void PrintAccountList(List<BankAccount> accounts)
    {
        Console.WriteLine("ID    TYPE       BALANCE");
        Console.WriteLine("----------------------------------------");
        foreach (var account in accounts)
        {
            Console.WriteLine(
                $"{account.AccountId,-5} {TypeName(account),-10} {account.Balance,10:C}");
        }
    }

    private void ShowAccounts()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("             MY ACCOUNTS");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var accounts = customerService.GetAccounts(customer.UserId);
        if (accounts.Count == 0)
            Console.WriteLine("You have no accounts.");
        else
            PrintAccountList(accounts);

        ConsoleHelper.Pause();
    }

    private BankAccount? SelectAccount(string title)
    {
        var accounts = customerService.GetAccounts(customer.UserId);
        if (accounts.Count == 0)
        {
            Console.WriteLine("You have no accounts.");
            ConsoleHelper.Pause();
            return null;
        }

        Console.WriteLine(title);
        Console.WriteLine("----------------------------------------");
        PrintAccountList(accounts);
        Console.WriteLine();

        int accountId = ConsoleHelper.ReadInt("Enter Account ID (0 to cancel): ");
        if (accountId == 0)
            return null;

        var selected = accounts.FirstOrDefault(a => a.AccountId == accountId);
        if (selected is null)
        {
            Console.WriteLine("That account doesn't belong to you or doesn't exist.");
            ConsoleHelper.Pause();
        }

        return selected;
    }

    private void HandleWithdraw()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("               WITHDRAW");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var account = SelectAccount("Select an account:");
        if (account is null) return;

        decimal amount = ConsoleHelper.ReadPositiveDecimal("Amount to withdraw: ");
        var result = customerService.Withdraw(account.AccountId, amount);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Withdrawal successful." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleDeposit()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("                DEPOSIT");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var account = SelectAccount("Select an account:");
        if (account is null) return;

        decimal amount = ConsoleHelper.ReadPositiveDecimal("Amount to deposit: ");
        var result = customerService.Deposit(account.AccountId, amount);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Deposit successful." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleTransfer()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("                TRANSFER");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var fromAccount = SelectAccount("Select the account to transfer FROM:");
        if (fromAccount is null) return;

        Console.WriteLine();
        int toAccountId = ConsoleHelper.ReadPositiveInt("Destination Account ID (yours or another customer's): ");
        decimal amount = ConsoleHelper.ReadPositiveDecimal("Amount to transfer: ");

        var result = customerService.Transfer(fromAccount.AccountId, toAccountId, amount);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Transfer successful." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleRecentTransactions()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("          RECENT TRANSACTIONS");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var account = SelectAccount("Select an account:");
        if (account is null) return;

        var transactions = customerService.GetRecentTransactions(account.AccountId, 5);

        Console.WriteLine();
        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions found.");
        }
        else
        {
            Console.WriteLine("DATE               TYPE         DIR   AMOUNT");
            Console.WriteLine("----------------------------------------");
            foreach (var t in transactions)
                Console.WriteLine(
                    $"{t.TransactionDate,-18:g} {t.TransactionType,-12} {t.Direction,-5} {t.TransactionAmount,10:C}");
        }

        ConsoleHelper.Pause();
    }

    private void HandleChequeBookRequest()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("          REQUEST CHEQUE BOOK");
        Console.WriteLine("========================================");
        Console.WriteLine();

        var account = SelectAccount("Select the checking account:");
        if (account is null) return;

        var result = customerService.RequestChequeBook(account.AccountId);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Cheque book request submitted." : result.Error);
        ConsoleHelper.Pause();
    }

    private void HandleChangePassword()
    {
        ConsoleHelper.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("             CHANGE PASSWORD");
        Console.WriteLine("========================================");
        Console.WriteLine();

        string currentPassword = ConsoleHelper.ReadPassword("Current password: ");
        string newPassword = ConsoleHelper.ReadPassword("New password: ");

        var result = customerService.ChangePassword(customer.UserId, currentPassword, newPassword);

        Console.WriteLine();
        Console.WriteLine(result.Success ? "Password changed successfully." : result.Error);
        ConsoleHelper.Pause();
    }
}