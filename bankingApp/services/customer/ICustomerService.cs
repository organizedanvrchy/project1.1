using bankingApp.common;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.transactions;

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
