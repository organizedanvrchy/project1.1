using bankingApp.common;
using bankingApp.data.entities.users;
using bankingApp.repositories.users;

namespace bankingApp.services.auth;

public interface IAuthService
{
    Result<User> Login(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepo;

    public AuthService(IUserRepository userRepository)
    {
        userRepo = userRepository;
    }

    public Result<User> Login(string username, string password)
    {
        var user = userRepo.GetByUsername(username.Trim().ToLower());
        if (user is null)
            return Result<User>.Fail("Invalid username or password.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Result<User>.Fail("Invalid username or password.");

        return Result<User>.Ok(user);
    }
}