using Cinema.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection s)
    {
        s.AddScoped<IGenreService, GenreService>();
        s.AddScoped<IMovieService, MovieService>();
        s.AddScoped<IHallService, HallService>();
        s.AddScoped<ISeatService, SeatService>();
        s.AddScoped<ISessionService, SessionService>();
        s.AddScoped<ICustomerService, CustomerService>();
        s.AddScoped<ITicketService, TicketService>();
        s.AddScoped<IPaymentService, PaymentService>();
        s.AddScoped<IReportService, ReportService>();
        s.AddScoped<IAuthService, AuthService>();
        s.AddScoped<ICustomerAuthService, CustomerAuthService>();
        s.AddScoped<ISeatMapService, SeatMapService>();
        s.AddSingleton<ISeatLockService>(_ => new InMemorySeatLockService(TimeProvider.System, TimeSpan.FromMinutes(5)));
        return s;
    }
}