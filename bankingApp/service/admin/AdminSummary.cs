namespace bankingApp.services.admin;

public class AdminSummary
{
    public List<CustomerAccountSummary> Customers { get; set; } = new();
    public int TotalAccountCount { get; set; }
    public decimal TotalBalance { get; set; }
    public int TotalTransactionCount { get; set; }
}