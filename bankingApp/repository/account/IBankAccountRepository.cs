using bankingApp.data.entities.accounts;

namespace bankingApp.repositories.account;

public interface IBankAccountRepository
{
    BankAccount? GetById(int accountId);
    List<BankAccount> GetByCustomer(int customerId);
    void Add(BankAccount account);
    void Save();   // persists whatever changes were made to a tracked entity
    void Delete(int accountId);
}