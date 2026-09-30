using Cinema.Domain.Entities;
using Cinema.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Web.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AuthService(IUserRepository repo) => _repo = repo;

    public async Task<AppUser?> ValidateCredentialsAsync(string login, string password)
    {
        var user = await _repo.GetByLoginAsync(login.Trim());
        if (user is null) return null;

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    public Task<bool> AnyAdminExistsAsync() => _repo.AnyAdminExistsAsync();

    public async Task CreateAdminAsync(string login, string password, string fullName)
    {
        var user = new AppUser { Login = login.Trim(), FullName = fullName, Role = "Admin", CreatedAt = DateTime.UtcNow };
        user.PasswordHash = _hasher.HashPassword(user, password);
        await _repo.AddAsync(user);
    }
}