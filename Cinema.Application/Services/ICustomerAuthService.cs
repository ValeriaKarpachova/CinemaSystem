using Cinema.Application.Models;
using Cinema.Domain.Common;
using Cinema.Domain.Entities;

namespace Cinema.Application.Services;

public interface ICustomerAuthService
{
    Task<OperationResult> RegisterAsync(RegisterCustomerRequest request);
    Task<Customer?> ValidateCredentialsAsync(string email, string password);
}