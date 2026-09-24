using bankingApp.data.entities.transactions;

namespace bankingApp.repositories.transactions;

public interface ITransactionRepository
{
    void Add(Transaction transaction);
    List<Transaction> GetAll();
    List<Transaction> GetByAccount(int accountId);
    List<Transaction> GetRecentByAccount(int accountId, int count);
}