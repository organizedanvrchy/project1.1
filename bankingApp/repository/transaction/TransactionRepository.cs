using bankingApp.data;
using bankingApp.data.entities.transactions;

namespace bankingApp.repositories.transactions;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext trContext;

    public TransactionRepository(BankingDbContext context)
    {
        trContext = context;
    }

    public void Add(Transaction transaction)
    {
        trContext.Transactions.Add(transaction);
        trContext.SaveChanges();
    }

    public List<Transaction> GetAll() =>
        trContext.Transactions
            .OrderByDescending(t => t.TransactionDate)
            .ToList();

    public List<Transaction> GetByAccount(int accountId) =>
        trContext.Transactions
            .Where(t => t.AccountId == accountId || t.RecipientAccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToList();

    public List<Transaction> GetRecentByAccount(int accountId, int count) =>
        trContext.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .ToList();
}