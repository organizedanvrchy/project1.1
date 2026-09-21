using bankingApp.data;
using bankingApp.data.entities.accounts;

namespace bankingApp.repositories.account;

public class BankAccountRepository : IBankAccountRepository
{
    private readonly BankingDbContext baContext;

    public BankAccountRepository(BankingDbContext context)
    {
        baContext = context;
    }

    // Find() both loads AND tracks the entity -- so once a service calls
    // account.Withdraw(...) on the object this returns, calling Save() below
    // is all that's needed to persist it. No separate method is required.
    public BankAccount? GetById(int accountId) =>
        baContext.BankAccounts.Find(accountId);

    public List<BankAccount> GetByCustomer(int customerId) =>
        baContext.BankAccounts.Where(a => a.CustomerId == customerId).ToList();

    public void Add(BankAccount account)
    {
        baContext.BankAccounts.Add(account);
        baContext.SaveChanges();
    }

    public void Save() => baContext.SaveChanges();

    public void Delete(int accountId)
    {
        var account = baContext.BankAccounts.Find(accountId);
        if (account is null)
            throw new InvalidOperationException($"Account {accountId} not found.");

        baContext.BankAccounts.Remove(account);
        baContext.SaveChanges();
    }
}