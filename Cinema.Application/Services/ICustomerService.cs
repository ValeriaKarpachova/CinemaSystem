using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Application.Services;

public interface ICustomerService
{
    Task<List<Customer>> SearchAsync(string? name);
    Task<OperationResult> SaveAsync(Customer customer);
    Task<OperationResult> DeleteAsync(int id);
}