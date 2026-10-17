using System.Text.RegularExpressions;
using Cinema.Application.Models;
using Cinema.Domain.Common;
using Cinema.Domain.Entities;
using Cinema.Domain.Exceptions;
using Cinema.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Application.Services;

public class CustomerAuthService : ICustomerAuthService
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private const string DuplicateMessage = "Користувач з такою поштою вже існує.";

    private readonly ICustomerRepository _repo;
    private readonly PasswordHasher<Customer> _hasher = new();

    public CustomerAuthService(ICustomerRepository repo) => _repo = repo;

    public async Task<OperationResult> RegisterAsync(RegisterCustomerRequest r)
    {
        var first = (r.FirstName ?? "").Trim();
        var last = (r.LastName ?? "").Trim();
        var phone = (r.Phone ?? "").Trim();
        var email = (r.Email ?? "").Trim().ToLowerInvariant();

        if (first.Length == 0 || last.Length == 0) return OperationResult.Fail("Вкажіть ім'я та прізвище.");
        if (first.Length > 50 || last.Length > 50) return OperationResult.Fail("Ім'я та прізвище до 50 символів.");
        if (phone.Length > 20) return OperationResult.Fail("Телефон до 20 символів.");
        if (!EmailRegex.IsMatch(email) || email.Length > 100) return OperationResult.Fail("Некоректна електронна пошта.");
        if (r.Password is null || r.Password.Length < 8 || r.Password.Length > 100)
            return OperationResult.Fail("Пароль має містити від 8 до 100 символів.");

        if (await _repo.EmailExistsAsync(email)) return OperationResult.Fail(DuplicateMessage);

        var customer = new Customer { FirstName = first, LastName = last, Phone = phone, Email = email };
        customer.PasswordHash = _hasher.HashPassword(customer, r.Password);

        try { await _repo.AddAsync(customer); }
        catch (DuplicateEmailException) { return OperationResult.Fail(DuplicateMessage); }

        return OperationResult.Ok();
    }

    public async Task<Customer?> ValidateCredentialsAsync(string email, string password)
    {
        var customer = await _repo.GetByEmailAsync((email ?? "").Trim().ToLowerInvariant());
        if (customer?.PasswordHash is null) return null;

        var result = _hasher.VerifyHashedPassword(customer, customer.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : customer;
    }
}