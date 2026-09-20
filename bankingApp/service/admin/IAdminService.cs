using bankingApp.common;
using bankingApp.data.entities.accounts;
using bankingApp.data.entities.chequebook;

namespace bankingApp.services.admin;

public interface IAdminService
{
    Result CreateCustomer(string username, string plainPassword);
    Result CreateBankAccount(int customerId, AccountType accountType, decimal initialBalance);
    Result DeleteCustomer(int customerId);
    Result DeleteBankAccount(int accountId);
    Result EditCustomerUsername(int customerId, string newUsername);
    AdminSummary GetSummary();
    Result ResetCustomerPassword(int customerId, string newPassword);
    List<ChequeBookRequest> GetPendingChequeBookRequests();
    Result ApproveChequeBookRequest(int requestId);
    Result RejectChequeBookRequest(int requestId);
    // Interface
}