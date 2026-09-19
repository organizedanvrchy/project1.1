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
            Console.WriteLine("===== CUSTOMER VIEW =====\n");
            Console.WriteLine($"Welcome Back, {customer.Username}!");
            Console.WriteLine("\n1. View My Accounts");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Recent Transactions");
            Console.WriteLine("6. Request Cheque Book");
            Console.WriteLine("7. Change Password");
            Console.WriteLine("8. Exit");
            Console.Write("\nPlease Select An Option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid Choice.");
                continue;
            }

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
                default: Console.WriteLine("Invalid Choice."); break;
            }
        }
    }

    // Displays the customer's accounts and returns the one they pick, or null if they back out.
    // Every action that needs "which account?" funnels through this, so the selection UI
    // only needs to be written once.
    private BankAccount? SelectAccount(string prompt)
    {
        var accounts = customerService.GetAccounts(customer.UserId);
        if (accounts.Count == 0)
        {
            Console.WriteLine("You have no accounts.");
            return null;
        }

        Console.WriteLine($"\n{prompt}");
        foreach (var account in accounts)
        {
            string typeName = account switch
            {
                CheckingAccount => "Checking",
                SavingsAccount => "Savings",
                LoanAccount => "Loan",
                _ => "Unknown"
            };
            Console.WriteLine($"  [{account.AccountId}] {typeName} - Balance: ${account.Balance:F2}");
        }

        Console.Write("Enter Account ID (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int accountId) || accountId == 0)
            return null;

        var selected = accounts.FirstOrDefault(a => a.AccountId == accountId);
        if (selected is null)
            Console.WriteLine("That account doesn't belong to you or doesn't exist.");

        return selected;
    }

    private void ShowAccounts()
    {
        SelectAccount("Your Accounts:"); // reuse the listing; ignore the returned selection here
    }

    private void HandleWithdraw()
    {
        var account = SelectAccount("Select an account to withdraw from:");
        if (account is null) return;

        Console.Write("Enter amount to withdraw: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        var result = customerService.Withdraw(account.AccountId, amount);
        Console.WriteLine(result.Success ? "Withdrawal successful." : result.Error);
    }

    private void HandleDeposit()
    {
        var account = SelectAccount("Select an account to deposit into:");
        if (account is null) return;

        Console.Write("Enter amount to deposit: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        var result = customerService.Deposit(account.AccountId, amount);
        Console.WriteLine(result.Success ? "Deposit successful." : result.Error);
    }

    private void HandleTransfer()
    {
        var fromAccount = SelectAccount("Select the account to transfer FROM:");
        if (fromAccount is null) return;

        // Destination can be another customer's account, so it's typed directly
        // rather than picked from this customer's own list.
        Console.Write("Enter destination Account ID (yours or another customer's): ");
        if (!int.TryParse(Console.ReadLine(), out int toAccountId))
        {
            Console.WriteLine("Invalid account ID.");
            return;
        }

        Console.Write("Enter amount to transfer: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        var result = customerService.Transfer(fromAccount.AccountId, toAccountId, amount);
        Console.WriteLine(result.Success ? "Transfer successful." : result.Error);
    }

    private void HandleRecentTransactions()
    {
        var account = SelectAccount("Select an account to view transactions for:");
        if (account is null) return;

        var transactions = customerService.GetRecentTransactions(account.AccountId, 5);
        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions found.");
            return;
        }

        foreach (var t in transactions)
            Console.WriteLine($"{t.TransactionDate:g} | {t.TransactionType} | ${t.TransactionAmount:F2}");
    }

    private void HandleChequeBookRequest()
    {
        var account = SelectAccount("Select the checking account to request a cheque book for:");
        if (account is null) return;

        var result = customerService.RequestChequeBook(account.AccountId);
        Console.WriteLine(result.Success ? "Cheque book request submitted." : result.Error);
    }

    private void HandleChangePassword()
    {
        Console.Write("Current password: ");
        string currentPassword = ConsoleHelper.ReadPassword();
        Console.Write("New password: ");
        string newPassword = ConsoleHelper.ReadPassword();

        var result = customerService.ChangePassword(customer.UserId, currentPassword, newPassword);
        Console.WriteLine(result.Success ? "Password changed successfully." : result.Error);
    }
}