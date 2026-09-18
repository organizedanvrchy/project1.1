using bankingApp.data.entities.accounts;

namespace bankingApp.data.entities.chequebook;

public class ChequeBookRequest
{
    public int RequestId { get; set; }
    public int AccountId { get; set; }
    public DateTime RequestDate { get; set; }
    public RequestStatus RequestStatus { get; set; }
    public DateTime? RequestApprovedDate { get; set; }
    public virtual BankAccount Account { get; set; } = null!;
}