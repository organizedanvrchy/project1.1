using bankingApp.data.entities.users;

namespace bankingApp.repositories.admin;

public interface IAdminRepository
{
    Admin? GetByUsername(string username);
    Admin? GetById(int adminId);
    void Add(Admin admin);
    bool Any();
}