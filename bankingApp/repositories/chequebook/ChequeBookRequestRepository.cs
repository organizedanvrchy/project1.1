using bankingApp.data;
using bankingApp.data.entities.chequebook;
using Microsoft.EntityFrameworkCore;

namespace bankingApp.repositories.chequebook;

public class ChequeBookRequestRepository : IChequeBookRequestRepository
{
    private readonly BankingDbContext cbContext;

    public ChequeBookRequestRepository(BankingDbContext context)
    {
        cbContext = context;
    }

    public void Add(ChequeBookRequest request)
    {
        cbContext.ChequeBookRequests.Add(request);
        cbContext.SaveChanges();
    }

    public List<ChequeBookRequest> GetPending() =>
        cbContext.ChequeBookRequests
            .Include(r => r.Account)
            .Where(r => r.RequestStatus == RequestStatus.Pending)
            .OrderBy(r => r.RequestDate)
            .ToList();

    public ChequeBookRequest? GetById(int requestId) =>
        cbContext.ChequeBookRequests.Find(requestId);

    public void Approve(int requestId)
    {
        var request = cbContext.ChequeBookRequests.Find(requestId);
        if (request is null)
            throw new InvalidOperationException($"Request {requestId} not found.");

        request.RequestStatus = RequestStatus.Approved;
        request.RequestApprovedDate = DateTime.Now;
        cbContext.SaveChanges();
    }

    public void Reject(int requestId)
    {
        var request = cbContext.ChequeBookRequests.Find(requestId);
        if (request is null)
            throw new InvalidOperationException($"Request {requestId} not found.");

        request.RequestStatus = RequestStatus.Rejected;
        cbContext.SaveChanges();
    }
}