using bankingApp.data.entities.accounts;
using bankingApp.data.entities.users;
using bankingApp.repositories.account;
using bankingApp.repositories.admin;
using bankingApp.repositories.customer;

namespace bankingApp.seeding;

public class DatabaseSeeder
{
    private readonly ICustomerRepository customerRepo;
    private readonly IAdminRepository adminRepo;
    private readonly IBankAccountRepository accountRepo;

    public DatabaseSeeder(
        ICustomerRepository customerRepository,
        IAdminRepository adminRepository,
        IBankAccountRepository accountRepository)
    {
        customerRepo = customerRepository;
        adminRepo = adminRepository;
        accountRepo = accountRepository;
    }

    public void Seed()
    {
        SeedAdmin();
        // SeedCustomers();
    }

    private void SeedAdmin()
    {
        if (adminRepo.Any())
            return;

        var admin = new Admin
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password")
        };

        adminRepo.Add(admin);
        Console.WriteLine("[Seed] Default admin created (username: admin / password: password)");
    }

    // private void SeedCustomers()
    // {
    //     if (customerRepo.Any())
    //         return;

    //     var customers = new List<Customer>
    //     {
    //         new() { Username = "alice", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1") },
    //         new() { Username = "bob",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2") },
    //         new() { Username = "carol", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password3") },
    //     };

    //     foreach (var customer in customers)
    //     {
    //         // Add() persists the customer and populates their generated UserId,
    //         // so bank accounts can reference it immediately afterward.
    //         customerRepo.Add(customer);
    //     }

    //     // Give each customer one of each account type so all three Withdraw/Deposit
    //     // overrides have real data to exercise from the CLI right away.
    //     SeedAccountsFor(customers[0], checkingBalance: 1500m, savingsBalance: 3000m, loanAmount: 10000m);
    //     SeedAccountsFor(customers[1], checkingBalance: 800m,  savingsBalance: 1200m, loanAmount: 5000m);
    //     SeedAccountsFor(customers[2], checkingBalance: 2200m, savingsBalance: 500m,  loanAmount: 15000m);

    //     Console.WriteLine("[Seed] 3 customers created, each with a Checking, Savings, and Loan account.");
    // }

    // private void SeedAccountsFor(Customer customer, decimal checkingBalance, decimal savingsBalance, decimal loanAmount)
    // {
    //     accountRepo.Add(new CheckingAccount
    //     {
    //         CustomerId = customer.UserId,
    //         Balance = checkingBalance
    //     });

    //     accountRepo.Add(new SavingsAccount
    //     {
    //         CustomerId = customer.UserId,
    //         Balance = savingsBalance,
    //         WithdrawalLimitPerMonth = 6,
    //         WithdrawalsThisPeriod = 0,
    //         PeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
    //     });

    //     accountRepo.Add(new LoanAccount
    //     {
    //         CustomerId = customer.UserId,
    //         Balance = loanAmount,          // full amount still owed
    //         OriginalLoanAmount = loanAmount
    //     });
    // }
}