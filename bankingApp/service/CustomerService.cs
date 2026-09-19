using bankingApp.common;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.chequebook;
using bankingApp.data.entities.transactions;
using bankingApp.repositories.account;
using bankingApp.repositories.chequebook;
using bankingApp.repositories.customer;
using bankingApp.repositories.transactions;

namespace bankingApp.services.customer;

public interface ICustomerService
{
    List<BankAccount> GetAccounts(int customerId);
    Result Withdraw(int accountId, decimal amount);
    Result Deposit(int accountId, decimal amount);
    Result Transfer(int fromAccountId, int toAccountId, decimal amount);
    List<Transaction> GetRecentTransactions(int accountId, int count);
    Result RequestChequeBook(int accountId);
    Result ChangePassword(int customerId, string currentPassword, string newPassword);
}

public class CustomerService : ICustomerService
{
    private readonly IBankAccountRepository accountRepo;
    private readonly ITransactionRepository transactionRepo;
    private readonly IChequeBookRequestRepository chequeBookRepo;
    private readonly ICustomerRepository customerRepo;

    public CustomerService(
        IBankAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IChequeBookRequestRepository chequeBookRepository,
        ICustomerRepository customerRepository)
    {
        accountRepo = accountRepository;
        transactionRepo = transactionRepository;
        chequeBookRepo = chequeBookRepository;
        customerRepo = customerRepository;
    }

    public List<BankAccount> GetAccounts(int customerId) =>
        accountRepo.GetByCustomer(customerId);

    public Result Withdraw(int accountId, decimal amount)
    {
        var account = accountRepo.GetById(accountId);
        if (account is null)
            return Result.Fail("Account not found.");

        // Each subtype decides its own rule -- CustomerService never branches on account type.
        var result = account.Withdraw(amount);
        if (!result.Success)
            return result;

        accountRepo.Save();
        transactionRepo.Add(new Transaction
        {
            AccountId = accountId,
            TransactionAmount = amount,
            TransactionType = TransactionType.Withdrawal,
            TransactionDate = DateTime.Now
        });

        return Result.Ok();
    }

    public Result Deposit(int accountId, decimal amount)
    {
        var account = accountRepo.GetById(accountId);
        if (account is null)
            return Result.Fail("Account not found.");

        var result = account.Deposit(amount);
        if (!result.Success)
            return result;

        accountRepo.Save();
        transactionRepo.Add(new Transaction
        {
            AccountId = accountId,
            TransactionAmount = amount,
            TransactionType = TransactionType.Deposit,
            TransactionDate = DateTime.Now
        });

        return Result.Ok();
    }

    public Result Transfer(int fromAccountId, int toAccountId, decimal amount)
    {
        if (fromAccountId == toAccountId)
            return Result.Fail("Cannot transfer to the same account.");

        var fromAccount = accountRepo.GetById(fromAccountId);
        if (fromAccount is null)
            return Result.Fail("Source account not found.");

        var toAccount = accountRepo.GetById(toAccountId);
        if (toAccount is null)
            return Result.Fail("Destination account not found.");

        var withdrawResult = fromAccount.Withdraw(amount);
        if (!withdrawResult.Success)
            return withdrawResult;

        var depositResult = toAccount.Deposit(amount);
        if (!depositResult.Success)
        {
            // Roll back the withdrawal in memory since the deposit side failed
            // (e.g. transferring into a Loan account for more than its remaining balance).
            fromAccount.Deposit(amount);
            return depositResult;
        }

        accountRepo.Save();
        transactionRepo.Add(new Transaction
        {
            AccountId = fromAccountId,
            RecipientAccountId = toAccountId,
            TransactionAmount = amount,
            TransactionType = TransactionType.Transfer,
            TransactionDate = DateTime.Now
        });

        return Result.Ok();
    }

    public List<Transaction> GetRecentTransactions(int accountId, int count) =>
        transactionRepo.GetRecentByAccount(accountId, count);

    public Result RequestChequeBook(int accountId)
    {
        var account = accountRepo.GetById(accountId);
        if (account is null)
            return Result.Fail("Account not found.");

        // Business rule lives here, not in the repository: only Checking accounts get chequebooks.
        if (account is not CheckingAccount)
            return Result.Fail("Cheque books are only available for checking accounts.");

        chequeBookRepo.Add(new ChequeBookRequest
        {
            AccountId = accountId,
            RequestDate = DateTime.Now,
            RequestStatus = RequestStatus.Pending
        });

        return Result.Ok();
    }

    public Result ChangePassword(int customerId, string currentPassword, string newPassword)
    {
        var customer = customerRepo.GetById(customerId);
        if (customer is null)
            return Result.Fail("Customer not found.");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, customer.PasswordHash))
            return Result.Fail("Current password is incorrect.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return Result.Fail("New password must be at least 6 characters.");

        customerRepo.UpdatePasswordHash(customerId, BCrypt.Net.BCrypt.HashPassword(newPassword));
        return Result.Ok();
    }
}