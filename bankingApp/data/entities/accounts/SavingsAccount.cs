using bankingApp.common;

namespace bankingApp.data.entities.accounts;

public class SavingsAccount : BankAccount
{
    private const int DefaultWithdrawalLimit = 6;

    // Nullable because these columns are NULL for every non-Savings row in the shared table.
    public int? WithdrawalLimitPerMonth { get; set; }
    public int? WithdrawalsThisPeriod { get; set; }
    public DateTime? PeriodStart { get; set; }

    public override Result Withdraw(decimal amount)
    {
        if (amount <= 0)
            return Result.Fail("Amount must be greater than zero.");

        ResetPeriodIfNeeded();

        int limit = WithdrawalLimitPerMonth ?? DefaultWithdrawalLimit;
        int used = WithdrawalsThisPeriod ?? 0;

        if (used >= limit)
            return Result.Fail($"Withdrawal limit of {limit} per month reached for this account.");

        if (Balance < amount)
            return Result.Fail("Insufficient funds.");

        Balance -= amount;
        WithdrawalsThisPeriod = used + 1;
        return Result.Ok();
    }

    // Rolls the counter over when a new calendar month starts. Kept on the entity itself
    // so the withdrawal check is self-contained and doesn't need a repository call.
    private void ResetPeriodIfNeeded()
    {
        var currentPeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        if (PeriodStart is null || currentPeriodStart > PeriodStart)
        {
            PeriodStart = currentPeriodStart;
            WithdrawalsThisPeriod = 0;
        }
    }
}