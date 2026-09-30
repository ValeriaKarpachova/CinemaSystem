using Cinema.Domain.Entities;

namespace Cinema.Web.Services;

public interface IAuthService
{
    Task<AppUser?> ValidateCredentialsAsync(string login, string password);
    Task<bool> AnyAdminExistsAsync();
    Task CreateAdminAsync(string login, string password, string fullName);
}