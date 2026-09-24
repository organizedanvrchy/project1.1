using bankingApp.common;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.chequebook;
using bankingApp.data.entities.users;
using bankingApp.repositories.account;
using bankingApp.repositories.admin;
using bankingApp.repositories.chequebook;
using bankingApp.repositories.customer;
using bankingApp.repositories.transactions;

namespace bankingApp.services.admin;

public class AdminService : IAdminService
{
    private readonly ICustomerRepository customerRepo;
    private readonly IAdminRepository adminRepo;
    private readonly IBankAccountRepository accountRepo;
    private readonly ITransactionRepository transactionRepo;
    private readonly IChequeBookRequestRepository chequeBookRepo;

    public AdminService(
        ICustomerRepository customerRepository,
        IAdminRepository adminRepository,
        IBankAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IChequeBookRequestRepository chequeBookRepository)
    {
        customerRepo = customerRepository;
        adminRepo = adminRepository;
        accountRepo = accountRepository;
        transactionRepo = transactionRepository;
        chequeBookRepo = chequeBookRepository;
    }

    // Creating a customer and creating their first bank account are two separate calls
    // (CreateCustomer, then CreateBankAccount) rather than one combined method -- since a
    // customer can hold several accounts, the CLI will call CreateCustomer once, then
    // CreateBankAccount as many times as the admin wants.
    public Result CreateCustomer(string username, string plainPassword)
    {
        username = username.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(username))
            return Result.Fail("Username is required.");

        if (customerRepo.GetByUsername(username) is not null)
            return Result.Fail("Username already taken.");

        if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
            return Result.Fail("Password must be at least 6 characters.");

        var newCustomer = new Customer
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword)
        };

        customerRepo.Add(newCustomer);
        return Result.Ok();
    }

        public Result CreateAdmin(string username, string plainPassword)
    {
        username = username.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(username))
            return Result.Fail("Username is required.");

        if (adminRepo.GetByUsername(username) is not null)
            return Result.Fail("Username already taken.");

        if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
            return Result.Fail("Password must be at least 6 characters.");

        var newAdmin = new Admin
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword)
        };

        adminRepo.Add(newAdmin);
        return Result.Ok();
    }

    public Result CreateBankAccount(int customerId, AccountType accountType, decimal initialBalance)
    {
        var customer = customerRepo.GetById(customerId);
        if (customer is null)
            return Result.Fail("Customer not found.");

        if (initialBalance < 0)
            return Result.Fail("Initial balance cannot be negative.");

        BankAccount account = accountType switch
        {
            AccountType.Checking => new CheckingAccount { CustomerId = customerId, Balance = initialBalance },
            AccountType.Savings => new SavingsAccount { CustomerId = customerId, Balance = initialBalance },
            AccountType.Loan => new LoanAccount { CustomerId = customerId, Balance = initialBalance, OriginalLoanAmount = initialBalance },
            _ => throw new ArgumentOutOfRangeException(nameof(accountType), "Unknown account type.")
        };

        accountRepo.Add(account);
        return Result.Ok();
    }

    public Result DeleteCustomer(int customerId)
    {
        var customer = customerRepo.GetById(customerId);
        if (customer is null)
            return Result.Fail("Customer not found.");

        customerRepo.Delete(customerId);
        return Result.Ok();
    }

    public Result DeleteBankAccount(int accountId)
    {
        var account = accountRepo.GetById(accountId);
        if (account is null)
            return Result.Fail("Account not found.");

        accountRepo.Delete(accountId);
        return Result.Ok();
    }

    public Result EditCustomerUsername(int customerId, string newUsername)
    {
        newUsername = newUsername.Trim().ToLower();

        var customer = customerRepo.GetById(customerId);
        if (customer is null)
            return Result.Fail("Customer not found.");

        if (customerRepo.GetByUsername(newUsername) is not null)
            return Result.Fail("That username is already taken.");

        customerRepo.UpdateUsername(customerId, newUsername);
        return Result.Ok();
    }

    public AdminSummary GetSummary()
    {
        var customers = customerRepo.GetAll();
        var transactions = transactionRepo.GetAll();

        var customerSummaries = new List<CustomerAccountSummary>();
        int totalAccounts = 0;
        decimal totalBalance = 0;
        decimal totalLoanBalance = 0;
        decimal totalNetBalance = 0;

        foreach (var customer in customers)
        {
            var accounts = accountRepo.GetByCustomer(customer.UserId);
            decimal customerTotal = accounts.Where(a => a is not LoanAccount).Sum(a => a.Balance);
            decimal customerLoanTotal = accounts.Where(a => a is LoanAccount).Sum(a => a.Balance);
            decimal netBalance = customerTotal - customerLoanTotal;

            customerSummaries.Add(new CustomerAccountSummary
            {
                CustomerId = customer.UserId,
                Username = customer.Username,
                AccountCount = accounts.Count,
                TotalBalance = customerTotal,
                TotalLoanBalance = customerLoanTotal,
                NetBalance = netBalance
            });

            totalAccounts += accounts.Count;
            totalBalance += customerTotal;
            totalLoanBalance += customerLoanTotal;
            totalNetBalance += netBalance;
        }

        return new AdminSummary
        {
            Customers = customerSummaries,
            TotalAccountCount = totalAccounts,
            TotalBalance = totalBalance,
            TotalLoanBalance = totalLoanBalance,
            TotalTransactionCount = transactions.Count,
            TotalNetBalance = totalNetBalance
        };
    }

    public Result ResetCustomerPassword(int customerId, string newPassword)
    {
        var customer = customerRepo.GetById(customerId);
        if (customer is null)
            return Result.Fail("Customer not found.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return Result.Fail("New password must be at least 6 characters.");

        customerRepo.UpdatePasswordHash(customerId, BCrypt.Net.BCrypt.HashPassword(newPassword));
        return Result.Ok();
    }

    public List<ChequeBookRequest> GetPendingChequeBookRequests() =>
        chequeBookRepo.GetPending();

    public Result ApproveChequeBookRequest(int requestId)
    {
        var request = chequeBookRepo.GetById(requestId);
        if (request is null)
            return Result.Fail("Request not found.");

        if (request.RequestStatus != RequestStatus.Pending)
            return Result.Fail("Request has already been processed.");

        chequeBookRepo.Approve(requestId);
        return Result.Ok();
    }

    public Result RejectChequeBookRequest(int requestId)
    {
        var request = chequeBookRepo.GetById(requestId);
        if (request is null)
            return Result.Fail("Request not found.");

        if (request.RequestStatus != RequestStatus.Pending)
            return Result.Fail("Request has already been processed.");

        chequeBookRepo.Reject(requestId);
        return Result.Ok();
    }
}