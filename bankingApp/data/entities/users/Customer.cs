using bankingApp.data.entities.accounts;

namespace bankingApp.data.entities.users;

public class Customer : User
{
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
}