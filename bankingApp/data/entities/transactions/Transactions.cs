using bankingApp.data.entities.accounts;

namespace bankingApp.data.entities.transactions;

public class Transaction
{
    public int TransactionId { get; set; }
    public decimal TransactionAmount { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public int AccountId { get; set; }
    public int? RecipientAccountId { get; set; }
    public virtual BankAccount Account { get; set; } = null!;
    public virtual BankAccount? RecipientAccount { get; set; }
}