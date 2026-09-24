using bankingApp.data.entities.users;

namespace bankingApp.repositories.users;

public interface IUserRepository
{
    User? GetByUsername(string username);
    User? GetById(int userId);
}