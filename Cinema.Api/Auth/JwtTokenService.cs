using System.Security.Claims;
using System.Text;
using Cinema.Api.Dtos;
using Cinema.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Cinema.Api.Auth;

public class JwtOptions
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int ExpiresMinutes { get; set; } = 60;
}

public class JwtTokenService(JwtOptions options)
{
    private readonly JsonWebTokenHandler _handler = new();

    public AuthResponse Create(Customer customer)
    {
        var fullName = $"{customer.FirstName} {customer.LastName}";
        var expires = DateTime.UtcNow.AddMinutes(options.ExpiresMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Issuer,
            Audience = options.Audience,
            Expires = expires,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.CustomerId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email ?? ""),
                new Claim("name", fullName),
                new Claim("role", "Customer")
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AuthResponse(_handler.CreateToken(descriptor), expires, fullName);
    }
}