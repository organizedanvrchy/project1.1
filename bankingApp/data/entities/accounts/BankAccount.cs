using bankingApp.common;
using bankingApp.data.entities.transactions;
using bankingApp.data.entities.users;

namespace bankingApp.data.entities.accounts;

public abstract class BankAccount
{
    public int AccountId { get; set; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    // Every subtype must decide its own withdrawal rule -- there's no sensible shared default.
    public abstract Result Withdraw(decimal amount);

    // Deposit has a sensible shared default; only LoanAccount needs to override it.
// BankAccount.cs
    public virtual Result Deposit(decimal amount)
    {
        if (amount <= 0)
            return Result.Fail("Amount must be greater than zero.");

        if (amount > TransactionLimits.MaxAmount)
            return Result.Fail($"Amount exceeds the maximum allowed of {TransactionLimits.MaxAmount:C}.");

        Balance += amount;
        return Result.Ok();
    }
}