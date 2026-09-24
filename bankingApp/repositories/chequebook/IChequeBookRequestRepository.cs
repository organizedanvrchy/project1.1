using bankingApp.data.entities.chequebook;

namespace bankingApp.repositories.chequebook;

public interface IChequeBookRequestRepository
{
    void Add(ChequeBookRequest request);
    List<ChequeBookRequest> GetPending();
    ChequeBookRequest? GetById(int requestId);
    void Approve(int requestId);
    void Reject(int requestId);
}