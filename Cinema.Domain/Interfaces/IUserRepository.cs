using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByLoginAsync(string login);
    Task<bool> AnyAdminExistsAsync();
    Task AddAsync(AppUser user);
}