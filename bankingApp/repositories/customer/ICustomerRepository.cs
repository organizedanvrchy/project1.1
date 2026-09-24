using bankingApp.data.entities.users;

namespace bankingApp.repositories.customer;

public interface ICustomerRepository
{
    Customer? GetById(int customerId);
    Customer? GetByUsername(string username);
    List<Customer> GetAll();
    void Add(Customer customer);
    void UpdateUsername(int customerId, string newUsername);
    void UpdatePasswordHash(int customerId, string newHash);
    void Delete(int customerId);
    bool Any();
}