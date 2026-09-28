using Cinema.Domain.Entities;
namespace Cinema.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<List<Customer>> SearchAsync(string? name);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}