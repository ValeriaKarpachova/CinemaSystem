using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Cinema.Web.Services;

public class CustomerService : ICustomerService
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo) => _repo = repo;

    public Task<List<Customer>> SearchAsync(string? name) => _repo.SearchAsync(name);

    public async Task<OperationResult> SaveAsync(Customer customer)
    {
        customer.FirstName = (customer.FirstName ?? "").Trim();
        customer.LastName = (customer.LastName ?? "").Trim();
        customer.Phone = (customer.Phone ?? "").Trim();
        customer.Email = (customer.Email ?? "").Trim();

        if (customer.FirstName.Length == 0) return OperationResult.Fail("Ім'я не може бути порожнім.");
        if (customer.LastName.Length == 0) return OperationResult.Fail("Прізвище не може бути порожнім.");
        if (customer.FirstName.Length > 50 || customer.LastName.Length > 50)
            return OperationResult.Fail("Ім'я та прізвище не можуть перевищувати 50 символів.");
        if (customer.Phone.Length > 20) return OperationResult.Fail("Телефон не може перевищувати 20 символів.");
        if (!EmailRegex.IsMatch(customer.Email)) return OperationResult.Fail("Некоректна електронна пошта.");
        if (customer.Email.Length > 100) return OperationResult.Fail("Email не може перевищувати 100 символів.");

        int? excludeId = customer.CustomerId == 0 ? null : customer.CustomerId;
        if (await _repo.EmailExistsAsync(customer.Email, excludeId))
            return OperationResult.Fail("Клієнт з такою поштою вже існує.");

        if (customer.CustomerId == 0) await _repo.AddAsync(customer);
        else await _repo.UpdateAsync(customer);

        return OperationResult.Ok();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return OperationResult.Ok();
        }
        catch (RelatedRecordsExistException)
        {
            return OperationResult.Fail("Неможливо видалити клієнта: у нього є квитки.");
        }
    }
}