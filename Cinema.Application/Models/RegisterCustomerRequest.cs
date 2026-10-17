namespace Cinema.Application.Models;

public record RegisterCustomerRequest(
    string FirstName, string LastName, string? Phone, string Email, string Password);