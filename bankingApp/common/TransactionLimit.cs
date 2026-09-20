namespace bankingApp.common;

public static class TransactionLimits
{
    // Matches the SQL column definition: decimal(19,4) -- 15 digits before the decimal point.
    public const decimal MaxAmount = 999_999_999_999_999.9999m;
}