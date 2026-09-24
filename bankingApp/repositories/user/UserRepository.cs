using bankingApp.data;
using bankingApp.data.entities.users;

namespace bankingApp.repositories.users;

public class UserRepository : IUserRepository
{
    private readonly BankingDbContext userContext;

    public UserRepository(BankingDbContext context)
    {
        userContext = context;
    }

    // Queries the base Users table -- EF reads the discriminator column and
    // hands back an actual Customer or Admin instance, not a generic User.
    public User? GetByUsername(string username) =>
        userContext.Users.FirstOrDefault(u => u.Username == username);

    public User? GetById(int userId) =>
        userContext.Users.Find(userId);
}