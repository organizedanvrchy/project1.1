using bankingApp.common;

namespace bankingApp.data.entities.accounts;

public class CheckingAccount : BankAccount
{
// CheckingAccount.cs
    public override Result Withdraw(decimal amount)
    {
        if (amount <= 0)
            return Result.Fail("Amount must be greater than zero.");

        if (amount > TransactionLimits.MaxAmount)
            return Result.Fail($"Amount exceeds the maximum allowed of {TransactionLimits.MaxAmount:C}.");

        if (Balance < amount)
            return Result.Fail("Insufficient funds.");

        Balance -= amount;
        return Result.Ok();
    }
}