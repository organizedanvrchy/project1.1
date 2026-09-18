using bankingApp.common;

namespace bankingApp.data.entities.accounts;

public class CheckingAccount : BankAccount
{
    public override Result Withdraw(decimal amount)
    {
        if (amount <= 0)
            return Result.Fail("Amount must be greater than zero.");

        if (Balance < amount)
            return Result.Fail("Insufficient funds.");

        Balance -= amount;
        return Result.Ok();
    }
}