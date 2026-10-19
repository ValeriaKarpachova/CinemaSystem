using Cinema.Api.Auth;
using Cinema.Api.Dtos;
using Cinema.Application.Models;
using Cinema.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ICustomerAuthService auth, JwtTokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCustomerRequest request)
    {
        var result = await auth.RegisterAsync(request);
        return result.Success
            ? StatusCode(StatusCodes.Status201Created)
            : Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var customer = await auth.ValidateCredentialsAsync(request.Email, request.Password);
        if (customer is null)
            return Problem(detail: "Невірна пошта або пароль.", statusCode: StatusCodes.Status401Unauthorized);

        return tokens.Create(customer);
    }
}