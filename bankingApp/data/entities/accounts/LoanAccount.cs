using bankingApp.common;

namespace bankingApp.data.entities.accounts;

public class LoanAccount : BankAccount
{
    // Nullable for the same reason as SavingsAccount's fields -- shared-table column.
    public decimal? OriginalLoanAmount { get; set; }

    // Balance represents the amount still owed, not funds available -- withdrawing
    // would mean "taking out more loan," which this design doesn't support.
    public override Result Withdraw(decimal amount) =>
        Result.Fail("Withdrawals are not permitted on a loan account.");

    // Depositing into a loan account means making a payment -- it reduces what's owed.
    public override Result Deposit(decimal amount)
    {
        if (amount <= 0)
            return Result.Fail("Amount must be greater than zero.");

        if (amount > Balance)
            return Result.Fail($"Payment exceeds remaining balance of ${Balance:F2}.");

        Balance -= amount;
        return Result.Ok();
    }
}