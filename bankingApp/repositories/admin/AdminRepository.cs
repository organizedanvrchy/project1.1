using bankingApp.data;
using bankingApp.data.entities.users;

namespace bankingApp.repositories.admin;

public class AdminRepository : IAdminRepository
{
    private readonly BankingDbContext adminContext;

    public AdminRepository(BankingDbContext context)
    {
        adminContext = context;
    }

    public Admin? GetByUsername(string username) =>
        adminContext.Set<Admin>().FirstOrDefault(a => a.Username == username);

    public Admin? GetById(int adminId) =>
        adminContext.Set<Admin>().Find(adminId);

    public void Add(Admin admin)
    {
        adminContext.Set<Admin>().Add(admin);
        adminContext.SaveChanges();
    }

    public bool Any() => adminContext.Set<Admin>().Any();
}