namespace bankingApp.services.admin;

public class CustomerAccountSummary
{
    public int CustomerId { get; set; }
    public string Username { get; set; } = null!;
    public int AccountCount { get; set; }
    public decimal TotalBalance { get; set; }
}