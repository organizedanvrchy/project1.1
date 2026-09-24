using bankingApp.data;
using bankingApp.data.entities.users;
using Microsoft.EntityFrameworkCore;

namespace bankingApp.repositories.customer;

public class CustomerRepository : ICustomerRepository
{
    private readonly BankingDbContext custContext;

    public CustomerRepository(BankingDbContext context)
    {
        custContext = context;
    }

    // Eager-loads BankAccounts since most callers need them right after fetching the customer
    // (e.g. displaying "your accounts" on login).
    public Customer? GetById(int customerId) =>
        custContext.Set<Customer>()
            .Include(c => c.BankAccounts)
            .FirstOrDefault(c => c.UserId == customerId);

    public Customer? GetByUsername(string username) =>
        custContext.Set<Customer>().FirstOrDefault(c => c.Username == username);

    public List<Customer> GetAll() =>
        custContext.Set<Customer>().ToList();

    public void Add(Customer customer)
    {
        custContext.Set<Customer>().Add(customer);
        custContext.SaveChanges();
    }

    public void UpdateUsername(int customerId, string newUsername)
    {
        var customer = custContext.Set<Customer>().Find(customerId);
        if (customer is null)
            throw new InvalidOperationException($"Customer {customerId} not found.");

        customer.Username = newUsername;
        custContext.SaveChanges();
    }

    public void UpdatePasswordHash(int customerId, string newHash)
    {
        var customer = custContext.Set<Customer>().Find(customerId);
        if (customer is null)
            throw new InvalidOperationException($"Customer {customerId} not found.");

        customer.PasswordHash = newHash;
        custContext.SaveChanges();
    }

    public void Delete(int customerId)
    {
        var customer = custContext.Set<Customer>().Find(customerId);
        if (customer is null)
            throw new InvalidOperationException($"Customer {customerId} not found.");

        custContext.Set<Customer>().Remove(customer);
        custContext.SaveChanges();
    }

    public bool Any() => custContext.Set<Customer>().Any();
}